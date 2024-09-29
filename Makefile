include ../Makefile.helpers
modname = CustomWeaponBehaviour
dependencies = CustomGrip BaseDamageModifiers HolyDamageManager TinyHelper

assemble:
	echo "Cannot assemble helper dll as standalone mod"
publish:
	echo "Cannot publish helper dll as standalone mod"
install:
	echo "Cannot install helper dll as standalone mod"
installothers:
	(cd ../Crusader && make install)
	(cd ../Juggernaut && make install)
	(cd ../MartialArtist && make install)
	(cd ../CustomMovesetPack && make install)
play:
	make installothers && (cd .. && make play)
