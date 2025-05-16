$TeamPing::Duration = 5;
$TeamPing::MaxRange = 2000;
$TeamPing::Cooldown = 12;

function remotePlacePingerLOS(%cl) {
    %player = Client::getOwnedObject(%cl);
    if (!isObject(%player) || Player::isDead(%player)) {
        Client::sendMessage(%cl, 0, "You must be alive to ping.");
        return;
    }

    %currentTime = getSimTime();
    if (%cl.lastPingerTime != "" && (%currentTime - %cl.lastPingerTime < $TeamPing::Cooldown)) {
        %timeLeft = $TeamPing::Cooldown - (%currentTime - %cl.lastPingerTime);
        Client::sendMessage(%cl, 0, "You must wait " @ floor(%timeLeft) + 1 @ " more seconds to ping again.");
        return;
    }

    if (GameBase::getLOSInfo(%player, $TeamPing::MaxRange)) {
        %targetPosition = $los::position;
        %pingerRotation = "0 0 0";

        %pingerObjectName = "LOS_Pinger_" @ %cl @ "_" @ getSimTime();

        %pinger = newObject(%pingerObjectName, StaticShape, PingerDevice, true);

        if (!isObject(%pinger)) {
            Client::sendMessage(%cl, 0, "Error: Failed to create pinger object.");
            echo("remotePlacePingerLOS: newObject failed for " @ %pingerObjectName);
            return;
        }
        echo("remotePlacePingerLOS: Pinger object " @ %pinger @ " created.");

        addToSet(MissionCleanup, %pinger);

        %playerTeam = GameBase::getTeam(%player);
        GameBase::setTeam(%pinger, %playerTeam);
        GameBase::setPosition(%pinger, %targetPosition);
        GameBase::setRotation(%pinger, %pingerRotation);
        GameBase::setMapName(%pinger, Client::getName(%cl) @ "'s Ping");

        if (isFunction("Pinger::onEnabled")) {
             Pinger::onEnabled(%pinger);
        } else {
             GameBase::setIsTarget(%pinger, true); // Fallback
             echo("remotePlacePingerLOS: Pinger::onEnabled not found, directly called GameBase::setIsTarget.");
        }

        Client::sendMessage(%cl, 0, "Location pinged for your team!");
        echo("remotePlacePingerLOS: Pinger " @ %pinger @ " placed by " @ %cl @ " at " @ %targetPosition);

        schedule("Rt::removePingMarker(" @ %pinger @ ");", $TeamPing::Duration);
        %cl.lastPingerTime = %currentTime;

    } else {
        Client::sendMessage(%cl, 0, "Ping out of range (Max: " @ $TeamPing::MaxRange @ "m) or no valid target in sight.");
    }
}

$Remote::PlacePingerLOS = "remotePlacePingerLOS";

function Rt::removePingMarker(%marker) {
    if (isObject(%marker)) {
        if (isFunction("Pinger::onDisabled")) {
            Pinger::onDisabled(%marker);
        } else {
            GameBase::setIsTarget(%marker, false); // Fallback
        }
        GameBase::startFadeOut(%marker);
        schedule("deleteObject(" @ %marker @ ");", 2.5);
    }
}