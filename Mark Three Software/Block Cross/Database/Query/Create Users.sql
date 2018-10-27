use Guardian
CREATE USER "ASPNET" FROM LOGIN [ARIEL\ASPNET];
CREATE USER "NT AUTHORITY" FROM LOGIN [NT AUTHORITY\SYSTEM];
CREATE USER "Carol Hill" FROM LOGIN [ARIEL\Carol Hill];
CREATE USER "Charles Rodriguez" FROM LOGIN [ARIEL\Charles Rodriguez];
CREATE USER "Elaine Russo" FROM LOGIN [ARIEL\Elaine Russo];
CREATE USER "Gary Brown" FROM LOGIN [ARIEL\Gary Brown];
CREATE USER "Gilbert Horowitz" FROM LOGIN [ARIEL\Gilbert Horowitz];
CREATE USER "Kareem Rao" FROM LOGIN [ARIEL\Kareem Rao];
CREATE USER "Kate Schwartz" FROM LOGIN [ARIEL\Kate Schwartz];
CREATE USER "Manny Cortez" FROM LOGIN [ARIEL\Manny Cortez];
CREATE USER "Roberto Garcia" FROM LOGIN [ARIEL\Roberto Garcia];
CREATE USER "Russell Jackson" FROM LOGIN [ARIEL\Russell Jackson];
CREATE USER "Susan Chung" FROM LOGIN [ARIEL\Susan Chung];
CREATE USER "Tony De Silva" FROM LOGIN [ARIEL\Tony De Silva];
CREATE USER "Victor Margolin" FROM LOGIN [ARIEL\Victor Margolin];
CREATE USER "Yoshio Shimura" FROM LOGIN [ARIEL\Yoshio Shimura];
CREATE USER "Guardian Operator" FROM LOGIN [ARIEL\Guardian Operator];
CREATE USER "Guardian Admin" FROM LOGIN [ARIEL\Guardian Admin];
go

execute sp_addrolemember db_owner, "ASPNET"
execute sp_addrolemember db_owner, "NT AUTHORITY"
execute sp_addrolemember db_owner, "Carol Hill"
execute sp_addrolemember db_owner, "Charles Rodriguez"
execute sp_addrolemember db_owner, "Elaine Russo"
execute sp_addrolemember db_owner, "Gary Brown"
execute sp_addrolemember db_owner, "Gilbert Horowitz"
execute sp_addrolemember db_owner, "Kareem Rao"
execute sp_addrolemember db_owner, "Kate Schwartz"
execute sp_addrolemember db_owner, "Manny Cortez"
execute sp_addrolemember db_owner, "Roberto Garcia"
execute sp_addrolemember db_owner, "Russell Jackson"
execute sp_addrolemember db_owner, "Susan Chung"
execute sp_addrolemember db_owner, "Tony De Silva"
execute sp_addrolemember db_owner, "Victor Margolin"
execute sp_addrolemember db_owner, "Yoshio Shimura"
execute sp_addrolemember db_owner, "Guardian Operator"
execute sp_addrolemember db_owner, "Guardian Admin"
go

