function PPinger::Ping(){
    remoteEval(2048, PlacePingerLOS);
}

function ping::bindinit() after GameBinds::Init{

  $GameBinds::CurrentMapHandle = GameBinds::GetActionMap2("playmap.sae");
  $GameBinds::CurrentMap = "playmap.sae";
  GameBinds::addBindCommand("Ping", "PPinger::Ping();", "");

}
