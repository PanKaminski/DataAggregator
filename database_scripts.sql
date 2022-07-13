-- api tasks table
create table api_tasks
(
    id INTEGER not null
    constraint pk_api_tasks primary key autoincrement,
    name TEXT not null,
    description TEXT,
    subscriber_id INTEGER references users on delete cascade,
    cron_time_expression TEXT not null
);
-- apis aggregators table
create table apis_aggregators
(
    id INTEGER not null constraint PK_apis_aggregators primary key,
    api_task_key INTEGER not null
        constraint fk_apis_aggregators_api_tasks_api_task_key
        references api_tasks
            on delete cascade,
    api_type TEXT not null
);
-- concrete coin api aggregator
create table coin_ranking_apis
(
    id INTEGER not null
        constraint PK_coin_ranking_apis
        primary key
        constraint fk_coin_ranking_apis_apis_aggregators_id
            references apis_aggregators
            on delete cascade,
    sparkline_time TEXT not null,
    reference_currency TEXT not null
);
-- covid tracker api table
create table covid_aggregator_apis
(
    id INTEGER not null
        constraint PK_covid_aggregator_apis
            primary key
        constraint fk_covid_aggregator_apis_apis_aggregators_id
            references apis_aggregators
            on delete cascade,
    country TEXT not null
);
-- weather tracker api table
create table weather_apis
(
    id INTEGER not null
        constraint PK_weather_apis
            primary key
        constraint fk_weather_apis_apis_aggregators_id
            references apis_aggregators
            on delete cascade,
    region TEXT not null
);
-- users table
create table users
(
    id INTEGER not null
        constraint pk_users
            primary key autoincrement,
    email TEXT not null,
    role TEXT not null,
    password_hash TEXT not null,
    count_of_requests INTEGER not null,
    registration_date TEXT not null
);
