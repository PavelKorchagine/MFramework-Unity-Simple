using UnityEngine;
using System.Collections;
using HighlightingSystem;

public class HighlighterInteractive : HighlighterBase, IHighlightingTarget
{
	#region MonoBehaviour
	// 
	protected override void Update()
	{
		base.Update();
	}
	#endregion

	#region IHighlightingTarget implementation
	// 
	public virtual void OnHighlightingFire1Down()
	{
		// Switch flashing
		//h.FlashingSwitch();
	}
	public virtual void OnHighlightingFire1Held() { }
	public virtual void OnHighlightingFire1Up() { }

	// 
	public virtual void OnHighlightingFire2Down() { }
	public virtual void OnHighlightingFire2Held() { }
	public virtual void OnHighlightingFire2Up()
	{
		// Switch seeThrough mode
		//h.seeThrough = !h.seeThrough;
	}

	// 
	public virtual void OnHighlightingMouseOver()
	{
		// Highlight object for one frame
		//h.On(Color.red);
	}
	#endregion
}