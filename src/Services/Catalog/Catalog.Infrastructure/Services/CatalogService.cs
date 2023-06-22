using AutoMapper;
using Catalog.Application.Dtos.ConcertDtos;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.ErrorModels;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Specification.ConcertSpecifications;

namespace Catalog.Infrastructure.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CatalogService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<IReadOnlyList<ConcertsShortViewDto>>> GetCurrentConcerts(ConcertsSpecParam concertsSpec, bool isDescOrder)
        {
            var spec = new ConcertsFilterAndSort(concertsSpec, isDescOrder);
            var concerts = await _unitOfWork.Repository<Concert>().ListAsync(spec);

            if (concerts == null)
            {
                return ResultReturnService.CreateErrorResult<IReadOnlyList<ConcertsShortViewDto>>
                    (ErrorStatusCode.NotFound, "No concerts");
            }

            var mappedConcerts = _mapper.Map<IReadOnlyList<ConcertsShortViewDto>>(concerts);

            return new Result<IReadOnlyList<ConcertsShortViewDto>>()
            {
                Value = mappedConcerts
            };
        }

        public async Task<Result<FullInfoConcertDto>> GetConcert(int id)
        {
            var spec = new ConcertFullInfo(id);
            var concert = await _unitOfWork.Repository<Concert>().GetEntityWithSpec(spec);

            if (concert == null)
            {
                return ResultReturnService.CreateErrorResult<FullInfoConcertDto>
                    (ErrorStatusCode.NotFound, "No concert with such id");
            }

            var mappedConcert = _mapper.Map<FullInfoConcertDto>(concert);

            return new Result<FullInfoConcertDto>()
            {
                Value = mappedConcert
            };
        }

        public async Task<Result<IReadOnlyList<ConcertsShortViewDto>>> GetSearchedConcerts(string searchTerm)
        {
            var spec = new ConcertsBySearchSpec(searchTerm);
            var concerts = await _unitOfWork.Repository<Concert>().ListAsync(spec);

            if (concerts == null)
            {
                return ResultReturnService.CreateErrorResult<IReadOnlyList<ConcertsShortViewDto>>
                    (ErrorStatusCode.NotFound, "No concerts");
            }

            var mappedConcerts = _mapper.Map<IReadOnlyList<ConcertsShortViewDto>>(concerts);

            return new Result<IReadOnlyList<ConcertsShortViewDto>>()
            {
                Value = mappedConcerts
            };
        }

        public async Task<Result> AddConcertAsync(FullInfoConcertDto fullInfoConcertModel)
        {
            var concert = _mapper.Map<Concert>(fullInfoConcertModel);

            await _unitOfWork.Repository<Concert>().Add(concert);
            var added = await _unitOfWork.Complete();
            if (added < 0)
            {
                return ResultReturnService.CreateErrorResult
                    (ErrorStatusCode.WrongAction, "Value cant be added to db");
            }

            return ResultReturnService.CreateSuccessResult();
        }

        public async Task<Result> DeleteConcertAsync(int concertId)
        {
            var spec = new ConcertWithValidTickets(concertId);
            var concert = await _unitOfWork.Repository<Concert>().GetEntityWithSpec(spec);

            if (concert != null)
            {
                return ResultReturnService.CreateErrorResult
                    (ErrorStatusCode.WrongAction, "Cant delete because of exsisting tickets");
            }

            var concertToDelete = await _unitOfWork.Repository<Concert>().GetByIdAsync(concertId);
            _unitOfWork.Repository<Concert>().Delete(concertToDelete);
            var deleted = await _unitOfWork.Complete();

            if (deleted < 0)
            {
                return ResultReturnService.CreateErrorResult
                    (ErrorStatusCode.WrongAction, "Value cant be deletd from db");
            }

            return ResultReturnService.CreateSuccessResult();
        }

        public async Task<Result> UpdateConcertAsync(FullInfoConcertDto concertFullInfo, int idConcert)
        {
            var concert = await _unitOfWork.Repository<Concert>().GetByIdAsync(idConcert);
            var updatedConcert = _mapper.Map(concertFullInfo, concert);
            _unitOfWork.Repository<Concert>().Update(updatedConcert);
            var updated = await _unitOfWork.Complete();

            if (updated < 0)
            {
                return ResultReturnService.CreateErrorResult
                    (ErrorStatusCode.WrongAction, "Value cant be updated in db");
            }

            return ResultReturnService.CreateSuccessResult();
        }

        public async Task<Result<IReadOnlyList<ConcertsShortViewDto>>> GetAllConcerts()
        {
            var spec = new ConcertFullInfo();
            var allConcerts = await _unitOfWork.Repository<Concert>().ListAsync(spec);

            if (allConcerts == null)
            {
                return ResultReturnService.CreateErrorResult<IReadOnlyList<ConcertsShortViewDto>>
                    (ErrorStatusCode.NotFound, "No concerts");
            }

            var mappedConcerts = _mapper.Map<IReadOnlyList<ConcertsShortViewDto>>(allConcerts);

            return new Result<IReadOnlyList<ConcertsShortViewDto>>()
            {
                Value = mappedConcerts
            };
        }
    }
}
