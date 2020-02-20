@echo off
title OpenSSL

cd\openssl\bin

if exist "C:\openssl\share\openssl.cnf" (

set OPENSSL_CONF=c:/openssl/share/openssl.cnf
start explorer.exe c:\openssl\bin

echo Welcome to OpenSSL

openssl

) else (

echo Error: openssl.cnf was not found
echo File openssl.cnf needs to be present in c:\openssl\share
pause

)

exit