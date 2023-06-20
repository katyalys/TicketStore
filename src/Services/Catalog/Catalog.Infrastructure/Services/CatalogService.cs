using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.ErrorModels;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Specification;
using Catalog.Domain.Specification.ConcertSpecifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Services
{
    public class CatalogService: ICatalogService
    {

        private readonly IUnitOfWork _unitOfWork;
        public CatalogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IReadOnlyList<Concert>>> GetCurrentConcerts(ConcertsSpecParam concertsSpec, bool isDescOrder)
        {
            var spec = new ConcertsFilterAndSort(concertsSpec, isDescOrder);
            var concerts = await _unitOfWork.Repository<Concert>().ListAsync(spec);

            if (concerts == null)
            {
                return ResultReturnService.CreateErrorResult<IReadOnlyList<Concert>>(ErrorStatusCode.NotFound, "No concerts");
            }

            return new Result<IReadOnlyList<Concert>>()
            {
                Value = concerts
            };
        }

        public async Task<Result<Concert>> GetConcert(int id)
        {
            var spec = new ConcertFullInfo(id);
            var concert = await _unitOfWork.Repository<Concert>().GetEntityWithSpec(spec);

            if (concert == null)
            {
                return ResultReturnService.CreateErrorResult<Concert>(ErrorStatusCode.NotFound, "No concert with such id");
            }

            return new Result<Concert>()
            {
                Value = concert
            };
        }

        public async Task<Result<IReadOnlyList<Concert>>> GetSearchedConcerts(string searchTerm)
        {
            var spec = new ConcertsBySearchSpec(searchTerm);
            var concerts = await _unitOfWork.Repository<Concert>().ListAsync(spec);

            if (concerts == null)
            {
                return ResultReturnService.CreateErrorResult<IReadOnlyList<Concert>>(ErrorStatusCode.NotFound, "No concerts");
            }

            return new Result<IReadOnlyList<Concert>>()
            {
                Value = concerts
            };
        }

        public async Task<Result> AddConcertAsync(Concert concert)
        {
            await _unitOfWork.Repository<Concert>().Add(concert);
            var added = await _unitOfWork.Complete();
            if (added < 0)
            {
                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "Value cant be added to db");
            }

            return ResultReturnService.CreateSuccessResult();
        }

        public async Task<Result> DeleteConcertAsync(int concertId)
        {
            var spec = new ConcertWithValidTickets(concertId);
            var concert = await _unitOfWork.Repository<Concert>().GetEntityWithSpec(spec);

            if (concert != null)
            {
                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "Cant delete because of exsisting tickets");
            }

            var concertToDelete = await _unitOfWork.Repository<Concert>().GetByIdAsync(concertId);
            _unitOfWork.Repository<Concert>().Delete(concertToDelete);
            var deleted = await _unitOfWork.Complete();

            if (deleted < 0)
            {
                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "Value cant be deletd from db");
            }

            return ResultReturnService.CreateSuccessResult();
        }

        public async Task<Result> UpdateConcertAsync(Concert concert)
        {
            _unitOfWork.Repository<Concert>().Update(concert);
            var updated = await _unitOfWork.Complete();

            if (updated < 0)
            {
                return ResultReturnService.CreateErrorResult(ErrorStatusCode.WrongAction, "Value cant be updated in db");
            }

            return ResultReturnService.CreateSuccessResult();
        }

        public async Task<Result<IReadOnlyList<Concert>>> GetAllConcerts()
        {
            var spec = new ConcertFullInfo();
            var allConcerts = await _unitOfWork.Repository<Concert>().ListAsync(spec);

            if (allConcerts == null)
            {
                return ResultReturnService.CreateErrorResult<IReadOnlyList<Concert>>(ErrorStatusCode.NotFound, "No concerts");
            }

            return new Result<IReadOnlyList<Concert>>()
            {
                Value = allConcerts
            };
        }
    }
}
