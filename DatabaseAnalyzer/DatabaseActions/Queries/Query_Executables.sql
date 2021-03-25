;WITH PF AS
(
SELECT 
object_id, 
name, 
schema_id,
'P' AS 'TYPE' 
FROM sys.procedures

UNION ALL

SELECT 
object_id,
name, 
schema_id, 
'F' AS 'TYPE'
FROM sys.all_objects WHERE TYPE IN ('IF','TF','FN')
)

SELECT
PF.TYPE 'TYPE',
PF.name AS 'NAME',
PF.schema_id 'SCHEMA_ID',
SCHMS.name AS 'SCHEMA_NAME',
CASE
	WHEN PARAMS.is_output = 1 THEN '@RETURNS'
	ELSE PARAMS.name
END AS 'PARAM_NAME',
CASE 
	WHEN PARAMS.system_type_id = PARAMS.user_type_id THEN SYSTYPE.name
	ELSE USRTYPE.name 
END AS 'TYPE_NAME',
CASE
	WHEN PARAMS.precision = 0 AND PARAMS.scale = 0 THEN CAST(PARAMS.max_length AS nvarchar(100))
	ELSE CONCAT(PARAMS.PRECISION,', ',PARAMS.scale)
END AS 'PRECISION',
PARAMS.is_nullable AS 'NULLABLE',
USRTYPE.is_table_type AS 'IS_TABLE'
FROM sys.all_parameters PARAMS

LEFT JOIN PF ON PF.object_id = PARAMS.object_id
JOIN sys.schemas SCHMS ON PF.schema_id = SCHMS.schema_id
JOIN sys.types SYSTYPE ON PARAMS.system_type_id = SYSTYPE.system_type_id AND PARAMS.system_type_id = SYSTYPE.user_type_id
LEFT JOIN sys.types USRTYPE ON PARAMS.system_type_id = USRTYPE.system_type_id AND PARAMS.user_type_id = USRTYPE.user_type_id

ORDER BY PF.object_id