# Variables
SOLUTION = uperapi.sln
SOLUTION_DIR = .
UNIT_TEST_PROJECT = $(SOLUTION_DIR)/Uper.UnitTest
INTEGRATION_TEST_PROJECT = $(SOLUTION_DIR)/Uper.IntegrationTest.*

# Default target
all: build

# Targets
build:
	dotnet build $(SOLUTION)

utest:
	dotnet test $(UNIT_TEST_PROJECT)

itest:
	dotnet test $(INTEGRATION_TEST_PROJECT)

clean:
	dotnet clean $(SOLUTION)

restore:
	dotnet restore $(SOLUTION)

run:
	dotnet run --project $(SOLUTION_DIR)/Uper.Api

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
	@echo "  utest        Run unit tests for the solution."
	@echo "  itest        Run integration tests for the solution."
	@echo "  clean        Clean the solution."
	@echo "  restore      Restore NuGet packages for the solution."
	@echo "  run          Run the API project."
	@echo "  format       Format the codebase."
	@echo "  lint         Verify formatting changes."
	@echo "  update-deps  Update NuGet dependencies (requires dotnet-outdated)."
	@echo "  help         Show this help message."

# Phony targets
.PHONY: all build test clean restore run format lint update-deps help
