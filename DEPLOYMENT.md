# Backend Deployment Configuration

## Environment Variables for Render Deployment

The following environment variables need to be set in your Render dashboard for the backend to work properly:

### JWT Configuration
```
JwtSettings__SecretKey=ThisIsAVeryLongSecretKeyThatIsDefinitelyLongEnoughForHMACSHA256AlgorithmAndShouldWorkProperlyInProductionEnvironment
JwtSettings__Issuer=QuantityMeasurementApp
JwtSettings__Audience=QuantityMeasurementAppUsers
JwtSettings__ExpiryMinutes=60
```

### Database Configuration (if using PostgreSQL)
Render automatically provides a `DATABASE_URL` environment variable for PostgreSQL databases. The application will automatically detect and use it.

### ASP.NET Core Configuration
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:10000
```

## Required Environment Variables Summary

| Variable | Value | Description |
|----------|-------|-------------|
| `JwtSettings__SecretKey` | `ThisIsAVeryLongSecretKeyThatIsDefinitelyLongEnoughForHMACSHA256AlgorithmAndShouldWorkProperlyInProductionEnvironment` | JWT signing key (must be at least 256 bits) |
| `JwtSettings__Issuer` | `QuantityMeasurementApp` | JWT token issuer |
| `JwtSettings__Audience` | `QuantityMeasurementAppUsers` | JWT token audience |
| `JwtSettings__ExpiryMinutes` | `60` | Token expiry time in minutes |

## Render Deployment Steps

1. Go to your Render dashboard
2. Select your QuantityMeasurementApp service
3. Go to Environment
4. Add the JWT environment variables listed above
5. Save and redeploy

## API Endpoints

Once deployed correctly, the following endpoints will be available:

### Authentication
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login

### Measurement Operations (require authentication)
- `POST /api/measurement/convert` - Convert between units
- `POST /api/measurement/compare` - Compare two quantities
- `POST /api/measurement/add` - Add quantities
- `POST /api/measurement/subtract` - Subtract quantities
- `POST /api/measurement/multiply` - Multiply quantities
- `POST /api/measurement/divide` - Divide quantities
- `GET /api/measurement/history` - Get measurement history

## Testing the API

You can test the API using curl:

```bash
# Register a user
curl -X POST https://your-render-url.com/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username":"test","password":"password123","email":"test@example.com","fullName":"Test User"}'

# Login
curl -X POST https://your-render-url.com/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"test","password":"password123"}'

# Use the token from login response for authenticated requests
curl -X POST https://your-render-url.com/api/measurement/convert \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -d '{"source":{"value":10,"unit":"FEET","measurementType":"Length"},"targetUnit":"INCH"}'
```