using SwiftlyS2.Shared.Events;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace SwiftlyS2.Core.SchemaDefinitions;

internal partial class CInButtonStateImpl : CInButtonState
{
  public GameButtonFlags ButtonPressed => (GameButtonFlags)ButtonStates[0];

  public GameButtonFlags ButtonChanged => (GameButtonFlags)ButtonStates[1];

  public GameButtonFlags ButtonScroll => (GameButtonFlags)ButtonStates[2];
  
}