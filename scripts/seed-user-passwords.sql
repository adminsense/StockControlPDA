-- Stock Control: test password Pda2!Stock (hashes are per-user).
-- Roles: admin = 1 (web), pda = 2 (PDA app)

UPDATE users SET Password = N'AQAAAAIAAYagAAAAENM7cNEPU0XZAiMR0XcY705PEzelzlRIcw+bQ8MnAAqhLuMXpmDCmIEmDpz/Vm507g==', Role = 1
WHERE Username = N'admin';

UPDATE users SET Password = N'AQAAAAIAAYagAAAAEMU13AnOAmBi32tMf9ZNQB5/x6NUm9sqUnyFSqpCghpxemFcQHV/KhNAaeN8IKqLvA==', Role = 2
WHERE Username = N'pda';

SELECT Id, Username, Name, Role, IsActive, Password
FROM users
WHERE Username IN (N'admin', N'pda')
ORDER BY Id;
