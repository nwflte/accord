@echo off
setlocal enabledelayedexpansion

echo.
echo Accord.NET Framework sample applications archive builder
echo =========================================================
echo. 
echo This Windows batch file will use WinRAR to automatically
echo build the compressed archives of the sample applications.
echo. 


:: Settings for complete and (libs-only) package creation
:: ---------------------------------------------------------
set rar="C:\Program Files\WinRAR\rar"
set opts=a -m5 -s
set sampleDir=..\..\Samples\
set outputDir=..\bin\Samples\
set sampleBin=\bin\x86\Release\

:: Get absolute sample dir
pushd .
cd %sampleDir%
set sampleDir=%CD%
popd

:: Get absolute output dir
pushd .
cd %outputDir%
set outputDir=%CD%
popd

echo.
echo  - Full sample path: %sampleDir%
echo  - Full output path: %outputDir%
echo.
echo  - WinRAR Command: %rar%
echo  - WinRAR Options: "%opts%"
echo.

pause

echo.
echo.
echo Packaging Accord.NET sample applications...
echo ---------------------------------------------------------


:: Create output directory
IF NOT EXIST %outputDir%\nul (
   mkdir %outputDir%
)

:: Remove old files
forfiles /p %outputDir% /m *.rar /c "cmd /c del @file"

:: For each sample application project
for /r %sampleDir% %%f in (*.csproj) do (
   
   :: Create the base filename (Accord-Assembly-ProjectName)
   set cur=%%~dpf
   set fileName=Accord!cur:%sampleDir%=!
   set fileName=!fileName:~0,-1!
   set fileName=!fileName:\=-!
   set fileName=!fileName!-bin.rar
   
   :: Convert the filename to lowercase
   for %%c in ("A=a" "B=b" "C=c" "D=d" "E=e" "F=f" "G=g" "H=h" "I=i" "J=j" "K=k" "L=l" "M=m" "N=n" "O=o" "P=p" "Q=q" "R=r" "S=s" "T=t" "U=u" "V=v" "W=w" "X=x" "Y=y" "Z=z") do set fileName=!fileName:%%~c!
   
   :: Get the binary files folder
   set folder=!cur!%sampleBin%
   
   :: Compress the files
   echo - Processing !fileName!
   pushd .
   cd !folder!
   %rar% %opts% -r %outputDir%\!fileName! *.* -x*.pdb -x*.xml
   popd
)


echo.
echo ---------------------------------------------------------
echo All sample applications have finished processing. 
echo ---------------------------------------------------------
echo.

pause