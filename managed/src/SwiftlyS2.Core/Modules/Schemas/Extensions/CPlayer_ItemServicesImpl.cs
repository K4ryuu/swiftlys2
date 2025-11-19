using SwiftlyS2.Core.EntitySystem;
using SwiftlyS2.Core.Natives;
using SwiftlyS2.Shared.Natives;
using SwiftlyS2.Shared.SchemaDefinitions;
using SwiftlyS2.Shared.Schemas;

namespace SwiftlyS2.Core.SchemaDefinitions;

internal partial class CPlayer_ItemServicesImpl
{

  public T GiveItem<T>() where T : ISchemaClass<T>
  {
    var name = T.ClassName;
    if (name == null)
    {
      throw new ArgumentException($"Can't give item with class {typeof(T).Name}, which doesn't have a designer name.");
    }
    return T.From(GameFunctions.CCSPlayer_ItemServices_GiveNamedItem(Address, name));
  }

  public T GiveItem<T>( string itemDesignerName ) where T : ISchemaClass<T>
  {
    return T.From(GameFunctions.CCSPlayer_ItemServices_GiveNamedItem(Address, itemDesignerName));
  }

  public void GiveItem( string itemDesignerName )
  {
    GameFunctions.CCSPlayer_ItemServices_GiveNamedItem(Address, itemDesignerName);
  }

  public void RemoveItems()
  {
    GameFunctions.CCSPlayer_ItemServices_RemoveWeapons(Address);
  }

  public void DropActiveItem()
  {
    GameFunctions.CCSPlayer_ItemServices_DropActiveItem(Address, Vector.Zero);
  }

}