@echo off
rm -r 1
rm -r 2
rm -r a
rm -r b

mkdir 1
mkdir 2

copy debug\*.exe 1\*.exe
copy debug\*.exe 2\*.exe

copy debug\*.dll 1\*.dll
copy debug\*.dll 2\*.dll

copy debug\*.config 1\*.config
copy debug\*.config 2\*.config

copy debug\config1.json 1\config.json
copy debug\config2.json 2\config.json

cd 1
start Distribox.CLI.exe

cd..
cd 2
start Distribox.CLI.exe