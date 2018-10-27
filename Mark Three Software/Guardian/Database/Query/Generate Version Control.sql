select char(9) + char(9) + 'union select ''' + quotename([sysobjects].[name], '''') + ''', 0'
from sysobjects
where [sysobjects].[name] not like 'dt_%' and
[sysobjects].[type] in ('U', 'P', 'TR', 'FN')
order by [sysobjects].[type], [sysobjects].[name]
