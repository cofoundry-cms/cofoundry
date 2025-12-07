#!/bin/bash

#########################################################
# Wait 60 seconds for SQL Server to start up 
# Adapted from https://github.com/microsoft/mssql-docker/blob/master/linux/preview/examples/mssql-customize/configure-db.sh
#########################################################

db_state=1
sqlcmd_error_code=1
num_attempts=0
max_attempts=60

while ([[ $db_state -ne 0 ]] || [[ $sqlcmd_error_code -ne 0 ]]) && [[ $num_attempts -lt $max_attempts ]]; do
	num_attempts=$((num_attempts+1))
	echo "Attempting to detect whether SQLServer is ready ($num_attempts of $max_attempts)"

	# Check system and user databases return "0" which means all databases are in an "online" state
	db_state=$(/opt/mssql-tools18/bin/sqlcmd -h -1 -t 1 -U sa -P $SA_PASSWORD -Q "SET NOCOUNT ON; select sum(state) from sys.databases" -C)
	
	# Check SQLCMD does not return an error code
	sqlcmd_error_code=$?

	sleep 1
done

if [[ $db_state -ne 0 ]] || [[ $sqlcmd_error_code -ne 0 ]]; then 
	echo "SQL Server took more than $max_attempts seconds to start up or one or more databases are not in an ONLINE state"
	exit 1
else
	echo "SQL Server is ready"
fi

#########################################################
# Restore databases from bacpac files
#########################################################

# Check to see if restore has already been run by querying the presence of cofoundry dbs
num_existing_dbs=$(/opt/mssql-tools18/bin/sqlcmd -h -1 -t 1 -U sa -P $SA_PASSWORD -Q "SET NOCOUNT ON; select count(*) from sys.databases where [name] like 'Cofoundry%'" -C) 
echo "Number of existing dbs: $num_existing_dbs" | xargs

# Only restore dbs on first run
if [[ $num_existing_dbs -lt 1 ]]; then
	echo "Restoring dbs"
	
	find /usr/config/backups -type f -name "*.bacpac" -print0 | while IFS= read -r -d '' bacpac_file; do
	
		backpac_file_name=${bacpac_file##*/};
		target_db="${backpac_file_name%.*}"

		echo "Restoring db '$bacpac_file' to '$target_db'"

		sqlpackage /Action:Import \
			/TargetServerName:. \
			/TargetUser:sa \
			/TargetPassword:$SA_PASSWORD \
			/SourceFile:$bacpac_file \
			/TargetDatabaseName:$target_db \
			/TargetTrustServerCertificate:True \
			/Quiet:True

	done
	echo "Dbs restored"
else 
	echo "$num_existing_dbs dbs found. Skipping db restore" | xargs
	exit 0
fi

echo "Db initialization complete"