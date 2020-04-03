using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SampleProj.Entities.Interfaces;
using SampleProj.Entities.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SampleProj.Repository
{
    public class RepositoryContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RepositoryContext(DbContextOptions<RepositoryContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var user = GetCurrentUserName();
            var role = GetCurrentUserRole();
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is ITrackable trackable)
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            trackable.Created = DateTime.UtcNow;
                            trackable.CreatedByName = user;
                            trackable.CreatedByRole = role;
                            break;

                        case EntityState.Added:
                            trackable.Created = DateTime.UtcNow;
                            trackable.CreatedByName = user;
                            trackable.CreatedByRole = role;
                            break;
                    }
                }
                else if (entry.Entity is ITrackableModify trackableModify)
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            trackableModify.Modified = DateTime.UtcNow;
                            trackableModify.ModifiedBy = user;
                            break;

                        case EntityState.Added:
                            trackableModify.Created = DateTime.UtcNow;
                            trackableModify.CreatedBy = user;
                            break;
                    }
                }
            }
        }

        private string GetCurrentUserName()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
                return httpContext.User.Identity.Name;
            else
                return null;
        }

        private string GetCurrentUserRole()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
                return httpContext.User.Claims?.Where(x => x.Type == ClaimTypes.Role)?.FirstOrDefault()?.Value;
            else
                return null;
        }
    }
}
