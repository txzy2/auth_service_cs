dps-dev:
	docker compose -f docker-compose.dev.yml ps

dbr-dev:
	docker compose -f docker-compose.dev.yml down && docker compose -f docker-compose.dev.yml up -d --build

dlogs-app:
	docker logs -f --tail=50 auth_cs_app

dInitMigrate-dev:
	docker exec -it auth_cs_app dotnet ef migrations add InitialCreate --output-dir Infrastructure/Data/Migrations

dMigrate-dev:
	docker exec -it auth_cs_app dotnet ef database update

dNewMigrate-dev:
	docker exec -it auth_cs_app dotnet ef migrations add $(name)

dRevertMigrate-dev:
	docker exec -it auth_cs_app dotnet ef database update $(name)

dRemoveMigrate-dev:
	docker exec -it auth_cs_app dotnet ef migrations remove

dListMigrations-dev:
	docker exec -it auth_cs_app dotnet ef migrations list

dDbDrop-dev:
	docker exec -it auth_cs_app dotnet ef database drop --force

dShell-app:
	docker exec -it auth_cs_app /bin/bash

.PHONY: dlogs-app dbr-dev dps-dev dInitMigrate-dev dMigrate-dev dNewMigrate-dev dRevertMigrate-dev dRemoveMigrate-dev dListMigrations-dev dDbDrop-dev dShell-app