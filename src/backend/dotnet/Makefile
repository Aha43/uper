# Variables
SOLUTION = uperapi.sln
PROJECT_DIR = src
TEST_PROJECT = $(PROJECT_DIR)/backend/dotnet/Uper.UnitTest

# Default target
all: build

# Targets
build:
	dotnet build $(SOLUTION)

test:
	dotnet test $(TEST_PROJECT)

clean:
	dotnet clean $(SOLUTION)

restore:
	dotnet restore $(SOLUTION)

run:
	dotnet run --project $(PROJECT_DIR)/backend/dotnet/Uper.Api

format:
	dotnet format $(SOLUTION)

lint:
	dotnet format --verify-no-changes $(SOLUTION)

update-deps:
	dotnet outdated -u

# Help target
help:
	@echo "Usage: make [target]"
	@echo "Targets:"
	@echo "  build        Build the solution."
	@echo "  test         Run tests for the solution."
	@echo "  clean        Clean the solution."
	@echo "  restore      Restore NuGet packages for the solution."
	@echo "  run          Run the API project."
	@echo "  format       Format the codebase."
	@echo "  lint         Verify formatting changes."
	@echo "  update-deps  Update NuGet dependencies (requires dotnet-outdated)."
	@echo "  help         Show this help message."

# Phony targets
.PHONY: all build test clean restore run format lint update-deps help
