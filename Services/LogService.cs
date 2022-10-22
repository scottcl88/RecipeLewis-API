using Database;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RecipeLewis.Models;

namespace RecipeLewis.Services
{
    public class LogService
    {
        private readonly ApplicationDbContext _dbContext;

        public LogService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private string TryConvertAdditionalData(object? requestData)
        {
            string? objStr = null;
            try
            {
                objStr = JsonConvert.SerializeObject(requestData);
            }
            catch (Exception ex)
            {
                objStr = "Object Serilization Failed" + " --- " + ex.ToString();
            }
            return objStr;
        }

        public async Task<bool> AddLog(NgxLog ngxLog)
        {
            var newLog = new Log()
            {
                CreatedDateTime = DateTime.UtcNow,
                ModifiedDateTime = DateTime.UtcNow,
                User = null,
                Additional = TryConvertAdditionalData(ngxLog.Additional),
                FileName = ngxLog.FileName,
                Level = ngxLog.Level,
                LineNumber = ngxLog.LineNumber,
                Message = ngxLog.Message,
                Timestamp = DateTime.Parse(ngxLog.Timestamp)
            };
            await _dbContext.AddAsync(newLog);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddLog(NgxLog ngxLog, UserId? userId)
        {
            if (userId == null)
            {
                userId = new UserId(0);
            }
            var user = _dbContext.Users.FirstOrDefault(x => x.UserId == userId.Value);

            var newLog = new Log()
            {
                CreatedDateTime = DateTime.UtcNow,
                ModifiedDateTime = DateTime.UtcNow,
                User = user,
                Additional = TryConvertAdditionalData(ngxLog.Additional),
                FileName = ngxLog.FileName,
                Level = ngxLog.Level,
                LineNumber = ngxLog.LineNumber,
                Message = ngxLog.Message,
                Timestamp = DateTime.Parse(ngxLog.Timestamp)
            };
            await _dbContext.AddAsync(newLog);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public bool Debug(string message, UserId? userId, object? requestData = null)
        {
            if (userId == null)
            {
                userId = new UserId(0);
            }
            var user = _dbContext.Users.FirstOrDefault(x => x.UserId == userId.Value);
            var newLog = new Log()
            {
                CreatedDateTime = DateTime.UtcNow,
                ModifiedDateTime = DateTime.UtcNow,
                Additional = TryConvertAdditionalData(requestData),
                Level = LoggerLevel.DEBUG,
                Message = message,
                Timestamp = DateTime.UtcNow,
                User = user
            };
            _dbContext.Add(newLog);
            _dbContext.SaveChanges();
            return true;
        }

        public bool Info(string message, UserId? userId, object? requestData = null)
        {
            if (userId == null)
            {
                userId = new UserId(0);
            }
            var user = _dbContext.Users.FirstOrDefault(x => x.UserId == userId.Value);
            var newLog = new Log()
            {
                CreatedDateTime = DateTime.UtcNow,
                ModifiedDateTime = DateTime.UtcNow,
                Additional = TryConvertAdditionalData(requestData),
                Level = LoggerLevel.INFO,
                Message = message,
                Timestamp = DateTime.UtcNow,
                User = user
            };
            _dbContext.Add(newLog);
            _dbContext.SaveChanges();
            return true;
        }

        public bool Error(string message, UserId? userId, object? requestData = null)
        {
            if (userId == null)
            {
                userId = new UserId(0);
            }
            var user = _dbContext.Users.FirstOrDefault(x => x.UserId == userId.Value);
            RejectChanges();
            var newLog = new Log()
            {
                CreatedDateTime = DateTime.UtcNow,
                ModifiedDateTime = DateTime.UtcNow,
                Additional = TryConvertAdditionalData(requestData),
                Level = LoggerLevel.ERROR,
                Message = message,
                Timestamp = DateTime.UtcNow,
                User = user
            };
            _dbContext.Add(newLog);
            _dbContext.SaveChanges();
            return true;
        }

        public bool Error(Exception exception, string message, UserId? userId, object? requestData = null)
        {
            if (userId == null)
            {
                userId = new UserId(0);
            }
            RejectChanges();
            var user = _dbContext.Users.FirstOrDefault(x => x.UserId == userId.Value);
            var newLog = new Log()
            {
                CreatedDateTime = DateTime.UtcNow,
                ModifiedDateTime = DateTime.UtcNow,
                User = user,
                Additional = TryConvertAdditionalData(requestData),
                Level = LoggerLevel.ERROR,
                Message = message + " --- " + exception.ToString(),
                Timestamp = DateTime.UtcNow
            };
            _dbContext.Add(newLog);
            _dbContext.SaveChanges();
            return true;
        }

        public void RejectChanges()
        {
            foreach (var entry in _dbContext.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified; //Revert changes made to deleted entity.
                        entry.State = EntityState.Unchanged;
                        break;

                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                }
            }
        }
    }
}