@echo off
@echo Deleting all BIN and OBJ folders...
for /d /r . %%d in (bin,obj,".vs") do @if exist "%%d" rd /s/q "%%d"
@echo .vs, BIN and OBJ folders successfully deleted :)

pause > nul