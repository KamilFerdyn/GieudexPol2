# GieudexPol Deployment Guide

## 🚀 Quick Start

To deploy the GieudexPol system quickly:

```bash
# Make the deployment script executable
chmod +x deploy.sh

# Run full deployment (build + start)
./deploy.sh
# Select option 7 for full deployment
```

## 📋 System Requirements

### Software Requirements
- **Docker** (v20.10+)
- **Docker Compose** (v1.29+)
- **.NET 8.0 SDK**
- **Node.js** (v18+)
- **npm** (v9+)

### Hardware Requirements
- **Minimum**: 2 CPU cores, 4GB RAM, 10GB disk space
- **Recommended**: 4 CPU cores, 8GB RAM, 20GB disk space

## 🛠️ Deployment Options

### Option 1: Docker Deployment (Recommended)

This is the easiest way to deploy the entire system with all dependencies.

#### Step 1: Build the system
```bash
./deploy.sh
# Select option 2: Build system
```

#### Step 2: Create database migrations
```bash
./deploy.sh
# Select option 3: Create database migrations
```

#### Step 3: Start the system
```bash
./deploy.sh
# Select option 4: Start system
```

### Option 2: Manual Deployment

#### 1. Build Frontend
```bash
cd GieudexPol.Frontend
npm install
npm run build --prod
cd ..
```

#### 2. Build Backend
```bash
dotnet restore GieudexPol.sln
dotnet build GieudexPol.sln -c Release
```

#### 3. Create Database Migrations
```bash
cd GieudexPol.API
dotnet ef migrations add InitialCreate --project ../GieudexPol.Infrastructure --startup-project . --output-dir Data/Migrations
cd ..
```

#### 4. Start with Docker Compose
```bash
docker-compose up -d
```

#### 5. Apply Database Migrations
```bash
docker-compose exec gieudexpol-api dotnet ef database update --project ../GieudexPol.Infrastructure --startup-project .
```

## 🌐 Accessing the System

After successful deployment:

- **Frontend**: `http://localhost`
- **Backend API**: `http://localhost:5000`
- **API Documentation**: `http://localhost:5000/swagger`
- **Database**: `localhost:1433`

## 📦 System Architecture

```
┌───────────────────────────────────────────────────────────────┐
│                        Client Browser                          │
└───────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌───────────────────────────────────────────────────────────────┐
│                        NGINX (Port 80/443)                     │
│  ┌─────────────────┐            ┌─────────────────────────────┐  │
│  │  Frontend (SPA) │            │  API Proxy (/api)          │  │
│  └─────────────────┘            └─────────────────────────────┘  │
└───────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌───────────────────────────────────────────────────────────────┐
│                        Backend API (Port 5000)                │
│  ┌─────────────────┐            ┌─────────────────────────────┐  │
│  │  .NET 8.0 API  │            │  Authentication (JWT)      │  │
│  └─────────────────┘            └─────────────────────────────┘  │
└───────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌───────────────────────────────────────────────────────────────┐
│                        MS SQL Server (Port 1433)              │
│  ┌─────────────────┐            ┌─────────────────────────────┐  │
│  │  Database       │            │  Entity Framework Core     │  │
│  └─────────────────┘            └─────────────────────────────┘  │
└───────────────────────────────────────────────────────────────┘
```

## 🔧 Configuration

### Environment Variables

The system uses the following environment variables (configured in `docker-compose.yml`):

- **Database Connection**:
  - `ConnectionStrings__DefaultConnection`: SQL Server connection string
  - `SA_PASSWORD`: SQL Server sa password (default: `YourStrong!Passw0rd`)

- **JWT Authentication**:
  - `Jwt__Key`: Secret key for JWT tokens (must be at least 32 characters)
  - `Jwt__Issuer`: JWT issuer (default: `GieudexPol`)
  - `Jwt__Audience`: JWT audience (default: `GieudexPolClient`)

### Customizing Configuration

1. **Database**: Edit `appsettings.json` and `docker-compose.yml`
2. **JWT Settings**: Edit `appsettings.json`
3. **Ports**: Edit `docker-compose.yml` and `nginx.conf`

## 🔄 Deployment Management

### Start System
```bash
docker-compose up -d
```

### Stop System
```bash
docker-compose down
```

### View Logs
```bash
docker-compose logs -f
```

### System Status
```bash
docker-compose ps
```

### Update System
```bash
./deploy.sh  # Select build and start options
docker-compose up -d --build
```

## 🛡️ Security Considerations

### Production Security Checklist

1. **Change all default passwords** in `docker-compose.yml` and `appsettings.json`
2. **Configure HTTPS** by adding SSL certificates to NGINX
3. **Set up proper CORS** policies in `Program.cs`
4. **Configure firewall** rules to allow only necessary ports
5. **Set up regular backups** for the database volume
6. **Monitor system logs** for suspicious activity
7. **Keep dependencies updated** with `npm audit` and `dotnet list package --vulnerable`

### JWT Security

- Use a strong secret key (at least 32 characters)
- Set appropriate token expiration times
- Implement token refresh mechanism
- Use HTTPS in production

## 🐛 Troubleshooting

### Common Issues

**Issue: Database connection failed**
- Solution: Check if SQL Server container is running (`docker-compose ps`)
- Verify connection string in `appsettings.json`
- Wait for database to fully initialize (may take 30-60 seconds)

**Issue: Frontend shows blank page**
- Solution: Check NGINX logs (`docker-compose logs gieudexpol-nginx`)
- Verify frontend build completed successfully
- Check browser console for errors

**Issue: API requests fail**
- Solution: Check if API container is running
- Verify CORS settings in `Program.cs`
- Check NGINX proxy configuration in `nginx.conf`

**Issue: Migration errors**
- Solution: Delete existing database volume (`docker volume rm gieudexpol-gieudexpol-data`)
- Recreate migrations (`dotnet ef migrations remove` then `dotnet ef migrations add`)

## 📈 Scaling the System

For production environments with higher traffic:

1. **Horizontal Scaling**:
   ```bash
   # Scale API instances
   docker-compose up -d --scale gieudexpol-api=3
   ```

2. **Database Optimization**:
   - Use SQL Server Enterprise Edition
   - Configure proper indexing
   - Set up read replicas

3. **Caching**:
   - Add Redis cache container
   - Configure caching in .NET API

4. **Load Balancing**:
   - Configure NGINX as load balancer
   - Add health checks

## 📚 Deployment Checklist

- [ ] Install all required software (Docker, .NET, Node.js)
- [ ] Configure environment variables
- [ ] Build frontend and backend
- [ ] Create database migrations
- [ ] Start containers with Docker Compose
- [ ] Apply database migrations
- [ ] Verify system is running (`docker-compose ps`)
- [ ] Test frontend and API endpoints
- [ ] Configure security settings
- [ ] Set up monitoring and backups

## 🎯 Next Steps

1. **Domain Configuration**: Set up a domain name and SSL certificates
2. **Monitoring**: Implement logging and monitoring (ELK stack, Prometheus)
3. **CI/CD Pipeline**: Set up automated deployment pipeline
4. **Backup Strategy**: Implement regular database backups
5. **Performance Testing**: Load test the system before production use

## 📞 Support

For deployment issues or questions:

1. Check the system logs: `docker-compose logs`
2. Review this deployment guide
3. Consult the main README.md for system architecture
4. Check Docker and .NET documentation for specific errors