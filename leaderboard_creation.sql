Use BHP;
CREATE TABLE leaderboardNA(
	id BIGINT PRIMARY KEY,
	username VARCHAR(20),
    elo DECIMAL(6,2) DEFAULT 2500.00);
CREATE TABLE matchesNA(
    id1 BIGINT,
    id2 BIGINT,
    username1 VARCHAR(20),
    username2 VARCHAR(20));
CREATE TABLE leaderboardEU(
	id BIGINT PRIMARY KEY,
	username VARCHAR(20),
    elo DECIMAL(6,2) DEFAULT 2500.00);
CREATE TABLE matchesEU(
    id1 BIGINT,
    id2 BIGINT,
    username1 VARCHAR(20),
    username2 VARCHAR(20));