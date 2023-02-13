﻿using University.Server.Domain.Models;
using University.Server.Domain.Repositories;
using University.Server.Domain.Services.Communication;

namespace University.Server.Domain.Services
{
    public class SemesterService : ISemesterService
    {
        private readonly ILogger<SemesterService> _logger;
        private readonly ISemesterRepository _semesterRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SemesterService(ILogger<SemesterService> logger, ISemesterRepository semesterRepository, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _semesterRepository = semesterRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<SemesterResponse> SaveAsync(Semester semester)
        {
            try
            {
                await _semesterRepository.AddAsync(semester);
                await _unitOfWork.CompleteAsync();

                return new SemesterResponse(semester);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new SemesterResponse($"An error occurred when saving the semester: {ex.Message}");
            }
        }
    }
}