#!/bin/bash

# GieudexPol Deployment Script
# This script automates the deployment of the GieudexPol system

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Function to display messages
function echo_info {
    echo -e "${GREEN}[INFO]${NC} $1"
}

function echo_warning {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

function echo_error {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to check if a command exists
function command_exists {
    command -v "$1" >/dev/null 2>&1
}

# Check requirements
function check_requirements {
    echo_info "Checking system requirements..."

    # Check Docker
    if ! command_exists docker; then
        echo_error "Docker is not installed. Please install Docker first."
        exit 1
    fi

    # Check Docker Compose
    if ! command_exists docker-compose; then
        echo_error "Docker Compose is not installed. Please install Docker Compose first."
        exit 1
    fi

    # Check .NET SDK
    if ! command_exists dotnet; then
        echo_error ".NET SDK is not installed. Please install .NET 8.0 SDK first."
        exit 1
    fi

    # Check Node.js
    if ! command_exists node; then
        echo_error "Node.js is not installed. Please install Node.js first."
        exit 1
    fi

    echo_info "All requirements are met!"
}

# Build the system
function build_system {
    echo_info "Building GieudexPol system..."

    # Build frontend
    echo_info "Building frontend..."
    cd GieudexPol.Frontend || { echo_error "Failed to enter frontend directory"; exit 1; }
    npm install
    npm run build --prod
    cd ..

    # Restore NuGet packages
    echo_info "Restoring NuGet packages..."
    dotnet restore GieudexPol.sln

    # Build backend
    echo_info "Building backend..."
    dotnet build GieudexPol.sln -c Release

    echo_info "Build completed successfully!"
}

# Create database migrations
function create_migrations {
    echo_info "Creating database migrations..."

    cd GieudexPol.API || { echo_error "Failed to enter API directory"; exit 1; }

    # Install EF Core tools if not installed
    if ! dotnet tool list -g | grep -q dotnet-ef; then
        echo_info "Installing EF Core tools..."
        dotnet tool install --global dotnet-ef
    fi

    # Create migration
    dotnet ef migrations add InitialCreate --project ../GieudexPol.Infrastructure --startup-project . --output-dir Data/Migrations

    echo_info "Migrations created successfully!"
    cd ..
}

# Start the system
function start_system {
    echo_info "Starting GieudexPol system..."

    # Start containers
    docker-compose up -d

    # Wait for database to be ready
    echo_info "Waiting for database to initialize..."
    sleep 30

    # Apply migrations
    echo_info "Applying database migrations..."
    docker-compose exec gieudexpol-api dotnet ef database update --project ../GieudexPol.Infrastructure --startup-project .

    echo_info "GieudexPol system is now running!"
    echo_info "Frontend: http://localhost"
    echo_info "Backend API: http://localhost:5000"
    echo_info "Database: localhost:1433"
}

# Stop the system
function stop_system {
    echo_info "Stopping GieudexPol system..."
    docker-compose down
    echo_info "System stopped successfully!"
}

# Show status
function show_status {
    echo_info "GieudexPol System Status:"
    docker-compose ps
}

# Main menu
function main_menu {
    echo_info "GieudexPol Deployment Script"
    echo_info "============================="
    echo_info "1. Check requirements"
    echo_info "2. Build system"
    echo_info "3. Create database migrations"
    echo_info "4. Start system"
    echo_info "5. Stop system"
    echo_info "6. Show status"
    echo_info "7. Full deployment (build + start)"
    echo_info "8. Exit"
    echo -n "Please select an option: "
    read option

    case $option in
        1) check_requirements ;;
        2) build_system ;;
        3) create_migrations ;;
        4) start_system ;;
        5) stop_system ;;
        6) show_status ;;
        7)
            check_requirements
            build_system
            create_migrations
            start_system
            ;;
        8) exit 0 ;;
        *) echo_error "Invalid option"; main_menu ;;
    esac
}

# Start the script
main_menu