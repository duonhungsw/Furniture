using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Furniture.Infrastructure;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
	public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
	{
		UpdateEntities(eventData.Context);
		return base.SavingChanges(eventData, result);
	}
	public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
	{
		UpdateEntities(eventData.Context);
		return base.SavingChangesAsync(eventData, result, cancellationToken);
	}

	private void UpdateEntities(DbContext? context)
	{
		if (context == null) return;

		foreach (var entry in context.ChangeTracker.Entries<IEntity>())
		{
			if (entry.State == EntityState.Added)
			{
				entry.Entity.CreatedAt = DateTime.UtcNow;
			}
			if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
			{
				entry.Entity.LastModified = DateTime.UtcNow;
			}
		}
	}
}
public static class Extensions
{
	//check xem những class reference properties trong IEntity có thay đổi không
	public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
		entry.References.Any(r =>
			r.TargetEntry != null &&
			r.TargetEntry.Metadata.IsOwned() &&
			(r.EntityEntry.State == EntityState.Added || r.EntityEntry.State == EntityState.Modified));
}
