
�
protocol.protopb"T
PBNetMsg
proto (	Rproto
payload (Rpayload
session (Rsession"B

PBLoginReq
account (	Raccount
password (	Rpassword"n

PBLoginRsp
uid (Ruid
name (	Rname
sceneId (RsceneId 
items (2
.pb.PBItemRitems"
PBEnterSceneReq"�
SceneObject
objId (RobjId
type (Rtype
hp (Rhp
maxHp (RmaxHp
mp (Rmp
maxMp (RmaxMp
	positionX (R	positionX
	positionY (R	positionY
	positionZ	 (R	positionZ
level
 (Rlevel"k
PBEnterSceneRsp3
sceneObjects (2.pb.SceneObjectRsceneObjects#
self (2.pb.SceneObjectRself"=
PBEnterSceneNotify'
object (2.pb.SceneObjectRobject"#
PBAttackReq
objId (RobjId"m
PBAttackRsp 
AttackObjId (RAttackObjId$

damage (Rdamage"p
PBAttackNotify 
AttackObjId (RAttackObjId$

damage (Rdamage")
PBObjectDieNotify
objId (RobjId"{
	PBMoveReq
	positionX (R	positionX
	positionY (R	positionY
	positionZ (R	positionZ
speed (Rspeed"�
PBMoveNotify
objId (RobjId
	positionX (R	positionX
	positionY (R	positionY
	positionZ (R	positionZ
speed (Rspeed"m
PBSyncPositionReq
	positionX (R	positionX
	positionY (R	positionY
	positionZ (R	positionZ"6
PBItem
itemId (RitemId
count (Rcount"<
PBUseItemReq
itemId (RitemId
count (Rcount"0
PBUseItemRsp 
items (2
.pb.PBItemRitemsBZ./;pbbproto3