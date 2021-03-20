;WITH TABLEVIEW AS
(
SELECT 
'T' as 'type',
object_id,
name,
schema_id,
type_desc
FROM sys.tables

UNION

SELECT 
'V' as 'type',
object_id,
name,
schema_id,
type_desc
FROM sys.views
)

SELECT
TABLEVIEW.TYPE 'TABLE_TYPE',
TABLEVIEW.name AS 'TABLE_NAME',
TABLEVIEW.schema_id 'SCHEMA_ID',
SCHMS.name AS 'SCHEMA_NAME',
COLS.name AS 'COLUMN_NAME',
CASE 
	WHEN COLS.system_type_id = COLS.user_type_id THEN SYSTYPE.name
	ELSE USRTYPE.name 
END AS 'TYPE_NAME',
CASE
	WHEN COLS.precision = 0 AND COLS.scale = 0 THEN CAST(COLS.max_length AS nvarchar(100))
	ELSE CONCAT(COLS.PRECISION,', ',COLS.scale)
END AS 'PRECISION',
COLS.is_nullable AS 'NULLABLE',
CASE
	WHEN COLS.default_object_id <> 0 THEN SUBSTRING(DEFVALUE.definition, 3, LEN(DEFVALUE.definition)-4)
	ELSE ''
END AS 'DEFAULT_VALUE'

FROM sys.all_columns COLS
JOIN TABLEVIEW ON TABLEVIEW.object_id = COLS.object_id
JOIN sys.schemas SCHMS ON TABLEVIEW.schema_id = SCHMS.schema_id
JOIN sys.types SYSTYPE ON COLS.system_type_id = SYSTYPE.system_type_id AND COLS.system_type_id = SYSTYPE.user_type_id
LEFT JOIN sys.types USRTYPE ON COLS.system_type_id = USRTYPE.system_type_id AND COLS.user_type_id = USRTYPE.user_type_id    
LEFT JOIN sys.default_constraints DEFVALUE ON COLS.default_object_id = DEFVALUE.object_id