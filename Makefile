modname = CustomWeaponBehaviour
gamepath = /mnt/c/Program\ Files\ \(x86\)/Steam/steamapps/common/Outward/Outward_Defed
pluginpath = BepInEx/plugins

assemble:
	echo "Cannot assemble helper dll as standalone mod"
publish:
	echo "Cannot publish helper dll as standalone mod"
install:
	echo "Cannot install helper dll as standalone mod"
clean:
	rm -f -r public
	rm -f $(modname).rar
	rm -f -r bin
info:
	echo Modname: $(modname)
play:
	(cd .. && make install && make play)
