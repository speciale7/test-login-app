# Full-Stack Login Application

A complete, production-ready full-stack login application with .NET 8 backend and Angular 18 frontend.

## 🚀 Features

### Authentication & Security
- ✅ JWT (JSON Web Token) authentication
- ✅ Refresh token functionality for enhanced security
- ✅ BCrypt password hashing
- ✅ Password complexity validation
- ✅ Protected routes with authentication guards
- ✅ Automatic token refresh on expiration
- ✅ Secure HTTP-only cookie option ready
- ✅ CORS configuration

### User Management
- ✅ User registration with validation
- ✅ User login with credential verification
- ✅ User profile display
- ✅ Logout functionality
- ✅ Token-based session management

### UI/UX
- ✅ Responsive design (mobile-friendly)
- ✅ Modern Material Design UI
- ✅ Password strength indicator
- ✅ Form validation with helpful error messages
- ✅ Loading indicators
- ✅ Toast notifications for user feedback

## 🛠️ Technology Stack

### Backend
- **.NET 8** - Modern web framework
- **ASP.NET Core Web API** - RESTful API
- **Entity Framework Core 8** - ORM
- **SQLite** - Lightweight database
- **BCrypt.Net** - Password hashing
- **JWT Bearer Authentication** - Token-based auth

### Frontend
- **Angular 18** - Modern web framework
- **TypeScript** - Type-safe JavaScript
- **Angular Material** - UI component library
- **RxJS** - Reactive programming
- **Angular Router** - Routing and navigation
- **Reactive Forms** - Form handling

### DevOps
- **Docker** - Containerization
- **Docker Compose** - Multi-container orchestration
- **Nginx** - Web server for frontend

## 📋 Prerequisites

### For Local Development
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) and npm
- Git

### For Docker Deployment
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

## 🔧 Installation & Setup

### Option 1: Local Development

#### Backend Setup

1. Navigate to the backend directory:
```bash
cd backend/LoginApi
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Update database connection (optional):
   - Edit `appsettings.json` to configure your settings
   - The default SQLite database will be created automatically

4. Run the backend:
```bash
dotnet run
```

The API will be available at `http://localhost:5000`

#### Frontend Setup

1. Navigate to the frontend directory:
```bash
cd frontend
```

2. Install dependencies:
```bash
npm install
```

3. Update API URL (if needed):
   - Edit `src/environments/environment.ts`
   - Set `apiUrl` to your backend URL (default: `http://localhost:5000/api`)

4. Run the development server:
```bash
npm start
```

The app will be available at `http://localhost:4200`

### Option 2: Docker Deployment

1. Make sure Docker and Docker Compose are installed

2. From the root directory, build and run with Docker Compose:
```bash
docker-compose up --build
```

3. Access the application:
   - **Frontend**: http://localhost
   - **Backend API**: http://localhost:5000

4. To stop the containers:
```bash
docker-compose down
```

5. To remove volumes (database data):
```bash
docker-compose down -v
```

## 📚 API Documentation

### Base URL
```
http://localhost:5000/api
```

### Endpoints

#### 1. Register User
```http
POST /auth/register
Content-Type: application/json

{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "abc123...",
  "expiresAt": "2024-01-01T12:00:00Z",
  "user": {
    "id": 1,
    "username": "johndoe",
    "email": "john@example.com",
    "createdAt": "2024-01-01T10:00:00Z"
  }
}
```

#### 2. Login
```http
POST /auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

**Response:** Same as register

#### 3. Refresh Token
```http
POST /auth/refresh
Content-Type: application/json

{
  "refreshToken": "abc123..."
}
```

**Response:** New access and refresh tokens

#### 4. Get Current User
```http
GET /auth/me
Authorization: Bearer {accessToken}
```

**Response:**
```json
{
  "id": 1,
  "username": "johndoe",
  "email": "john@example.com",
  "createdAt": "2024-01-01T10:00:00Z"
}
```

#### 5. Logout
```http
POST /auth/logout
Authorization: Bearer {accessToken}
Content-Type: application/json

{
  "refreshToken": "abc123..."
}
```

**Response:**
```json
{
  "message": "Logged out successfully"
}
```

## 🏗️ Project Structure

```
test-login-app/
├── backend/
│   └── LoginApi/
│       ├── Controllers/        # API endpoints
│       │   └── AuthController.cs
│       ├── Data/              # Database context
│       │   └── AppDbContext.cs
│       ├── DTOs/              # Data transfer objects
│       │   └── AuthDtos.cs
│       ├── Models/            # Domain models
│       │   ├── User.cs
│       │   └── RefreshToken.cs
│       ├── Services/          # Business logic
│       │   ├── IAuthService.cs
│       │   └── AuthService.cs
│       ├── Program.cs         # App configuration
│       ├── appsettings.json   # Configuration
│       └── Dockerfile
│
├── frontend/
│   └── src/
│       ├── app/
│       │   ├── components/    # UI components
│       │   │   ├── login/
│       │   │   ├── register/
│       │   │   ├── dashboard/
│       │   │   └── navigation/
│       │   ├── services/      # Angular services
│       │   │   └── auth.service.ts
│       │   ├── guards/        # Route guards
│       │   │   ├── auth.guard.ts
│       │   │   └── guest.guard.ts
│       │   ├── interceptors/  # HTTP interceptors
│       │   │   └── auth.interceptor.ts
│       │   ├── models/        # TypeScript interfaces
│       │   │   └── auth.model.ts
│       │   ├── app.component.ts
│       │   ├── app.routes.ts
│       │   └── app.config.ts
│       ├── environments/      # Environment configs
│       └── index.html
│
├── docker-compose.yml
├── .gitignore
└── README.md
```

## ⚙️ Configuration

### Backend Configuration (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=loginapp.db"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyForJWTTokenGenerationAndValidation2024!",
    "Issuer": "LoginApi",
    "Audience": "LoginApiClients",
    "AccessTokenExpirationMinutes": "15",
    "RefreshTokenExpirationDays": "7"
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:4200",
      "http://localhost:80"
    ]
  }
}
```

### Frontend Configuration (environment.ts)

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'
};
```

## 🔒 Security Features

### Implemented
- ✅ **Password Hashing**: BCrypt with salt (cost factor 12)
- ✅ **JWT Tokens**: Secure token generation and validation
- ✅ **Refresh Tokens**: Separate refresh token mechanism
- ✅ **CORS**: Configured to allow specific origins
- ✅ **Input Validation**: Server and client-side validation
- ✅ **SQL Injection Prevention**: EF Core parameterized queries
- ✅ **XSS Protection**: Angular's built-in sanitization
- ✅ **Password Complexity**: Enforced minimum requirements

### Best Practices
- Change the JWT SecretKey in production
- Use HTTPS in production environments
- Implement rate limiting for authentication endpoints
- Regular security audits and dependency updates
- Store sensitive data in environment variables
- Implement account lockout after failed attempts (future enhancement)

## 🧪 Testing

### Backend
```bash
cd backend/LoginApi
dotnet test
```

### Frontend
```bash
cd frontend
npm test
```

## 📝 Environment Variables

### Backend (.env or appsettings)
- `ConnectionStrings__DefaultConnection`: Database connection string
- `JwtSettings__SecretKey`: Secret key for JWT signing
- `JwtSettings__Issuer`: JWT issuer
- `JwtSettings__Audience`: JWT audience
- `JwtSettings__AccessTokenExpirationMinutes`: Access token lifetime
- `JwtSettings__RefreshTokenExpirationDays`: Refresh token lifetime

### Frontend (environment.ts)
- `apiUrl`: Backend API base URL

## 🐛 Troubleshooting

### Common Issues

**Issue**: Cannot connect to backend from frontend
- **Solution**: Check CORS settings in `appsettings.json`
- Verify backend is running on the correct port
- Check `apiUrl` in frontend environment file

**Issue**: Database errors on first run
- **Solution**: The database is created automatically
- Ensure write permissions in the backend directory
- Check SQLite is properly installed

**Issue**: JWT token expired
- **Solution**: The app automatically refreshes tokens
- Clear localStorage and login again if issues persist

**Issue**: Docker build fails
- **Solution**: Ensure Docker daemon is running
- Check Docker Compose version compatibility
- Verify all Dockerfiles are present

## 🚀 Deployment

### Production Checklist
- [ ] Change JWT SecretKey to a strong, random value
- [ ] Enable HTTPS
- [ ] Update CORS origins for production domain
- [ ] Set `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Use a production-grade database (PostgreSQL, SQL Server)
- [ ] Implement rate limiting
- [ ] Set up monitoring and logging
- [ ] Configure automated backups
- [ ] Review and update security headers

## 📄 License

This project is licensed under the MIT License.

## 👥 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📧 Support

For issues and questions, please open an issue on GitHub.

---

**Built with ❤️ using .NET 8 and Angular 18**
