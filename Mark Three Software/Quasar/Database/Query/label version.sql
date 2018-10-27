declare @tag_name sysname
set @tag_name = 'Version 1.0'

select char(9) + char(9) + 'Union select ''' + quotename(@tag_name, '""') + ''', ''' +
	quotename("VersionControl"."Name", '""') + ''', ' + convert(varchar, "VersionControl"."Revision")
from "VersionControl"
order by "VersionControl"."Type", "VersionControl"."Name"


