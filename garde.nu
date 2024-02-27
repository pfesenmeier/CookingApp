use std log;

let project_sln = "CookingApp.sln";
let project_path = (
    [$nu.home-path '**' $project_sln] 
    | path join
    | ls $in
    | first
    | get name
    | path expand
    | path dirname
)

let db_compose_path = [$project_path docker db docker-compose.yml] | path join

log info $"Project Path: ($project_path)"
log info $"Database compose path ($db_compose_path)"

# psql
$env.PGUSER = 'postgres'
$env.PGPASSWORD = 'postgres'
$env.PGHOST = '0.0.0.0'
$env.PGPORT = '5432'
$env.PGDATABASE = 'db'

# dadbod
# note: https://stackoverflow.com/a/11545725
$env.DATABASE_URL = $"postgres://($env.PGUSER):($env.PGPASSWORD)@($env.PGHOST):($env.PGPORT)/($env.PGDATABASE)"

# starts the dev db
def "db start" [] {
  docker compose "-f" $db_compose_path up
}


