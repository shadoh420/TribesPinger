//================================================================================================
// Pinger System - pinger.cs
//================================================================================================
StaticShapeData PingerDevice
{
	className = "Pinger";
	shapeFile = "breath";
	castLOS = false;
	supression = false;
	mapFilter = 2;
   mapIcon = "M_marker";
	visibleToSensor = true;       // show name on map


    disableCollision = true;
};

function Pinger::onEnabled(%this)
{
   // Called when the pinger should become active/visible
   GameBase::setIsTarget(%this, true); // Makes it show up on IFF/map for the team
   //echo("Pinger " @ %this @ " enabled (setIsTarget true)");
}

function Pinger::onDisabled(%this)
{
   // Called when the pinger should become inactive/invisible
   GameBase::setIsTarget(%this, false);
   //echo("Pinger " @ %this @ " disabled (setIsTarget false)");
}

function Pinger::onDestroyed(%this) // Called when deleteObject() happens or it's destroyed
{
   Pinger::onDisabled(%this); // Ensure it's no longer a target
   //echo("Pinger " @ %this @ " destroyed");
}