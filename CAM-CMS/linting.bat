@echo off
cls
echo Linting... & echo: & echo [93mStarting with CSS linting...[0m
@echo off
for /F "tokens=*" %%F in ('npx stylelint src --fix') do SET cssResult=%%F

if "%cssResult%" == "" (
    echo [92mCSS linting complete with no errors found or all errors were fixed![0m
) else (
    echo [91mCSS linting complete with "%cssResult%" left![0m

    setlocal EnableDelayedExpansion
    choice /n /m "Do you want to see the errors? (Y/[N])"
    if !ERRORLEVEL! EQU 1 goto :CSSERRORS else goto :JSLINT

    :CSSERRORS
    call npx stylelint src
    pause
    goto :JSLINT
)

:JSLINT
echo:
echo [93mMoving to JS linting...[0m

@echo off
for /F "tokens=*" %%F in ('npx eslint src --fix') do SET jsResult=%%F

if "%jsResult%"=="" (
    echo [92mJS linting complete with no errors found or all errors were fixed![0m
) else (
    echo [91mJS linting complete with "%jsResult:~4%" left![0m

    call npx eslint src
    goto :END
)

:END
echo:

echo Linting done!