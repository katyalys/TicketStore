using AutoMapper;
using Catalog.Application.Dtos.ConcertDtos;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.ErrorModels;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Specification.ConcertSpecifications;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CatalogService> _logger;

        public CatalogService(IUnitOfWork unitOfWork, IMapper mapper,
            ILogger<CatalogService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IReadOnlyList<ConcertsShortViewDto>>> GetCurrentConcertsAsync(ConcertsSpecParam concertsSpec, bool isDescOrder)
        {
            var spec = new ConcertsFilterAndSort(concertsSpec, isDescOrder);
            var concerts = await _unitOfWork.Repository<Concert>().ListAsync(spec);

            if (!concerts.Any())
            {
                _logger.LogWarning("No current concerts were found");

                return ResultReturnService.CreateErrorResult<IReadOnlyList<ConcertsShortViewDto>>
                    (ErrorStatusCode.NotFound, "No concerts");
            }

            var mappedConcerts = _mapper.Map<IReadOnlyList<ConcertsShortViewDto>>(concerts);

            _logger.LogInformation("Current concerts were successfully received. Count: {Count}", mappedConcerts.Count);

            return new Result<IReadOnlyList<ConcertsShortViewDto>>()
            {
                Value = mappedConcerts
            };
        }

        public async Task<Result<FullInfoConcertDto>> GetConcertAsync(int id)
        {
            var spec = new ConcertFullInfo(id);
            var concert = await _unitOfWork.Repository<Concert>().GetEntityWithSpecAsync(spec);

            if (concert == null)
            {
                _logger.LogWarning("No concert found with id {ConcertId}", id);

                return ResultReturnService.CreateErrorResult<FullInfoConcertDto>
                    (ErrorStatusCode.NotFound, "No concert with such id");
            }

            var mappedConcert = _mapper.Map<FullInfoConcertDto>(concert);

            _logger.LogInformation("Concert with id {ConcertId} was successfully recieved", id);

            return new Result<FullInfoConcertDto>()
            {
                Value = mappedConcert
            };
        }

        public async Task<Result<IReadOnlyList<ConcertsShortViewDto>>> GetSearchedConcertsAsync(string searchTerm)
        {
            var spec = new ConcertsBySearchSpec(searchTerm);
            var concerts = await _unitOfWork.Repository<Concert>().ListAsync(spec);

            if (!concerts.Any())
            {
                _logger.LogWarning("No concerts found for search term {SearchTerm}", searchTerm);

                return ResultReturnService.CreateErrorResult<IReadOnlyList<ConcertsShortViewDto>>
                    (ErrorStatusCode.NotFound, "No concerts");
            }

            var mappedConcerts = _mapper.Map<IReadOnlyList<ConcertsShortViewDto>>(concerts);

            _logger.LogInformation("Concerts with search term {SearchTerm} were successfully recieved", searchTerm);

            return new Result<IReadOnlyList<ConcertsShortViewDto>>()
            {
                Value = mappedConcerts
            };
        }

        public async Task<Result> AddConcertAsync(FullInfoConcertDto fullInfoConcertModel)
        {
            var concert = _mapper.Map<Concert>(fullInfoConcertModel);

            await _unitOfWork.Repository<Concert>().AddAsync(concert);
            var added = await _unitOfWork.CompleteAsync();

            if (added < 0)
            {
                _logger.LogError("Failed to add concert to the database");

                return ResultReturnService.CreateErrorResult
                    (ErrorStatusCode.WrongAction, "Value cant be added to db");
            }

            _logger.LogInformation("Concert was successfully added to database");

            return ResultReturnService.CreateSuccessResult();
        }

        public async Task<Result> DeleteConcertAsync(int concertId)
        {
            var spec = new ConcertWithValidTickets(concertId);
            var concert = await _unitOfWork.Repository<Concert>().GetEntityWithSpecAsync(spec);

            if (concert != null)
            {
                _logger.LogWarning("Cannot delete concert with id {ConcertId} because of existing tickets", concertId);

                return ResultReturnService.CreateErrorResult
                    (ErrorStatusCode.WrongAction, "Cant delete because of exsisting tickets");
            }

            var concertToDelete = await _unitOfWork.Repository<Concert>().GetByIdAsync(concertId);
            _unitOfWork.Repository<Concert>().Delete(concertToDelete);
            var deleted = await _unitOfWork.CompleteAsync();

            if (deleted < 0)
            {
                _logger.LogError("Failed to delete concert with id {ConcertId} from the database", concertId);

                return ResultReturnService.CreateErrorResult
                    (ErrorStatusCode.WrongAction, "Value cant be deletd from db");
            }

            _logger.LogInformation("Concert with id {ConcertId} was successfully deleted from database", concertId);

            return ResultReturnService.CreateSuccessResult();
        }

        public async Task<Result> UpdateConcertAsync(FullInfoConcertDto concertFullInfo, int idConcert)
        {
            var concert = await _unitOfWork.Repository<Concert>().GetByIdAsync(idConcert);
            var updatedConcert = _mapper.Map(concertFullInfo, concert);
            _unitOfWork.Repository<Concert>().Update(updatedConcert);
            var updated = await _unitOfWork.CompleteAsync();

            if (updated < 0)
            {
                _logger.LogError("Failed to update concert with id {ConcertId} in the database", idConcert);

                return ResultReturnService.CreateErrorResult
                    (ErrorStatusCode.WrongAction, "Value cant be updated in db");
            }

            _logger.LogInformation("Concert with id {ConcertId} was successfully updated in database", idConcert);

            return ResultReturnService.CreateSuccessResult();
        }

        public async Task<Result<IReadOnlyList<ConcertsShortViewDto>>> GetAllConcertsAsync()
        {
            var spec = new ConcertFullInfo();
            var allConcerts = await _unitOfWork.Repository<Concert>().ListAsync(spec);

            if (!allConcerts.Any())
            {
                _logger.LogWarning("No concerts were found");

                return ResultReturnService.CreateErrorResult<IReadOnlyList<ConcertsShortViewDto>>
                    (ErrorStatusCode.NotFound, "No concerts");
            }

            var mappedConcerts = _mapper.Map<IReadOnlyList<ConcertsShortViewDto>>(allConcerts);

            _logger.LogInformation("All concerts were successfully recieved");

            return new Result<IReadOnlyList<ConcertsShortViewDto>>()
            {
                Value = mappedConcerts
            };
        }
    }
}
