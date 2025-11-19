using Microsoft.Extensions.Logging;
using SwiftlyS2.Core.Extensions;
using SwiftlyS2.Core.Natives;
using SwiftlyS2.Core.NetMessages;
using SwiftlyS2.Core.SchemaDefinitions;
using SwiftlyS2.Shared.EntitySystem;
using SwiftlyS2.Shared.Natives;
using SwiftlyS2.Shared.Profiler;
using SwiftlyS2.Shared.SchemaDefinitions;
using SwiftlyS2.Shared.Schemas;
using System.Collections.Frozen;

namespace SwiftlyS2.Core.EntitySystem;

internal class EntitySystemService : IEntitySystemService, IDisposable
{
  private List<EntityOutputHookCallback> _callbacks = new();
  private Lock _lock = new();
  private ILoggerFactory _loggerFactory;
  private IContextedProfilerService _profiler;

  public EntitySystemService( ILoggerFactory loggerFactory, IContextedProfilerService profiler )
  {
    _loggerFactory = loggerFactory;
    _profiler = profiler;
  }

  public T CreateEntity<T>() where T : class, ISchemaClass<T>
  {
    var designerName = GetEntityDesignerName<T>();
    if (designerName == null)
    {
      throw new ArgumentException($"Can't create entity with class {typeof(T).Name}, which doesn't have a designer name.");
    }
    return CreateEntityByDesignerName<T>(designerName);
  }

  public T CreateEntityByDesignerName<T>( string designerName ) where T : ISchemaClass<T>
  {
    var handle = NativeEntitySystem.CreateEntityByName(designerName);
    if (handle == nint.Zero)
    {
      throw new ArgumentException($"Failed to create entity by designer name: {designerName}, probably invalid designer name.");
    }
    return T.From(handle);
  }

  public CHandle<T> GetRefEHandle<T>( T entity ) where T : class, ISchemaClass<T>
  {
    var handle = NativeEntitySystem.GetEntityHandleFromEntity(entity.Address);
    return new CHandle<T> {
      Raw = handle,
    };
  }

  public CCSGameRules? GetGameRules()
  {
    var handle = NativeEntitySystem.GetGameRules();
    return handle.IsValidPtr() ? new CCSGameRulesImpl(handle) : null;
  }

  public IEnumerable<CEntityInstance> GetAllEntities()
  {
    CEntityIdentity? pFirst = new CEntityIdentityImpl(NativeEntitySystem.GetFirstActiveEntity());

    for (; pFirst != null && pFirst.IsValid; pFirst = pFirst.Next)
    {
      yield return new CEntityInstanceImpl(pFirst.Address.Read<nint>());
    }
  }

  public IEnumerable<T> GetAllEntitiesByClass<T>() where T : class, ISchemaClass<T>
  {
    var designerName = GetEntityDesignerName<T>();
    if (designerName == null)
    {
      throw new ArgumentException($"Can't get entities with class {typeof(T).Name}, which doesn't have a designer name");
    }
    return GetAllEntities().Where(( entity ) => entity.Entity?.DesignerName == designerName).Select(( entity ) => T.From(entity.Address));
  }

  public IEnumerable<T> GetAllEntitiesByDesignerName<T>( string designerName ) where T : class, ISchemaClass<T>
  {
    return GetAllEntities()
      .Where(entity => entity.Entity?.DesignerName == designerName)
      .Select(entity => T.From(entity.Address));
  }

  public T? GetEntityByIndex<T>( uint index ) where T : class, ISchemaClass<T>
  {
    var handle = NativeEntitySystem.GetEntityByIndex(index);
    if (handle == nint.Zero)
    {
      return null;
    }
    return T.From(handle);
  }

  Guid IEntitySystemService.HookEntityOutput<T>( string outputName, IEntitySystemService.EntityOutputHandler callback )
  {
    var hook = new EntityOutputHookCallback(GetEntityDesignerName<T>() ?? "", outputName, callback, _loggerFactory, _profiler);
    lock (_lock)
    {
      _callbacks.Add(hook);
    }
    return hook.Guid;
  }

  public void UnhookEntityOutput( Guid guid )
  {
    lock (_lock)
    {
      _callbacks.RemoveAll(callback =>
      {
        if (callback.Guid == guid)
        {
          callback.Dispose();
          return true;
        }
        return false;
      });
    }
  }

  private string? GetEntityDesignerName<T>() where T : class, ISchemaClass<T>
  {
    return T.ClassName;
  }

  public void Dispose()
  {
    lock (_lock)
    {
      foreach (var callback in _callbacks)
      {
        callback.Dispose();
      }
      _callbacks.Clear();
    }
  }
}
