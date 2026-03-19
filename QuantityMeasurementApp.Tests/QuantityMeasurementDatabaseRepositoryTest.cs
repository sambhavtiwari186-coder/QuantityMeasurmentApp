using System;
using System.Linq;
using NUnit.Framework;
using Microsoft.Data.SqlClient;
using QuantityMeasurementApp.Entity;
using QuantityMeasurementApp.Repository;

namespace QuantityMeasurementApp.Tests
{
    // UC16 Database Repository Tests.
    // These tests require that schema.sql has been run in SSMS first and
    // that a QuantityMeasurementTestDB database (separate from production) exists.
    //
    // To create the test database, run the following SQL in SSMS:
    //   CREATE DATABASE QuantityMeasurementTestDB;
    //   USE QuantityMeasurementTestDB;
    //   -- then paste the CREATE TABLE block from schema.sql
    [TestFixture]
    public class QuantityMeasurementDatabaseRepositoryTest
    {
        // ── Connection string pointing to the TEST database ────────────────
        private const string TestConnectionString =
            @"Data Source=.\SQLEXPRESS;Database=QuantityMeasurementTestDB;" +
            "Integrated Security=True;TrustServerCertificate=True;";

        private QuantityMeasurementDatabaseRepository _repo = null!;

        [OneTimeSetUp]
        public void CreateTestDatabase()
        {
            // Ensure the test database and table exist before any test runs.
            // This is idempotent — safe to run multiple times.
            string masterConn = @"Data Source=.\SQLEXPRESS;Database=master;" +
                                "Integrated Security=True;TrustServerCertificate=True;";

            using var conn = new SqlConnection(masterConn);
            conn.Open();

            ExecuteNonQuery(conn,
                "IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'QuantityMeasurementTestDB') " +
                "CREATE DATABASE QuantityMeasurementTestDB;");

            conn.ChangeDatabase("QuantityMeasurementTestDB");

            ExecuteNonQuery(conn,
                "IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.QuantityMeasurements') AND type = N'U') " +
                "CREATE TABLE dbo.QuantityMeasurements (" +
                "  Id              INT IDENTITY(1,1) NOT NULL PRIMARY KEY," +
                "  FirstOperand    NVARCHAR(500)  NOT NULL," +
                "  SecondOperand   NVARCHAR(500)  NOT NULL DEFAULT N'N/A'," +
                "  OperationType   NVARCHAR(100)  NOT NULL," +
                "  MeasurementType NVARCHAR(100)  NOT NULL DEFAULT N'N/A'," +
                "  FinalResult     NVARCHAR(500)  NOT NULL," +
                "  HasError        BIT            NOT NULL DEFAULT 0," +
                "  ErrorMessage    NVARCHAR(1000) NOT NULL DEFAULT N'None'," +
                "  RecordedAt      DATETIME2(7)   NOT NULL DEFAULT SYSUTCDATETIME()" +
                ");");
        }

        [SetUp]
        public void SetUp()
        {
            _repo = new QuantityMeasurementDatabaseRepository(TestConnectionString);
            
            // Start each test with a clean table
            using var conn = new SqlConnection(TestConnectionString);
            conn.Open();
            ExecuteNonQuery(conn, "DELETE FROM dbo.QuantityMeasurements;");
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after each test
            using var conn = new SqlConnection(TestConnectionString);
            conn.Open();
            ExecuteNonQuery(conn, "DELETE FROM dbo.QuantityMeasurements;");
        }

        // ─── Helper ──────────────────────────────────────────────────────
        private static QuantityMeasurementEntity MakeLengthAddEntity() =>
            new("1 Feet", "12 Inch", "Addition", "2 Feet", "Length");

        private static void ExecuteNonQuery(SqlConnection conn, string sql)
        {
            using var cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        // ─── Tests ───────────────────────────────────────────────────────

        [Test]
        public void TestSaveMeasurement_PersistsToDatabase()
        {
            _repo.SaveMeasurement(MakeLengthAddEntity());

            int count = _repo.GetTotalCount();
            Assert.That(count, Is.EqualTo(1), "Expected 1 row after saving one entity.");
        }

        [Test]
        public void TestGetAllMeasurements_ReturnsAllSaved()
        {
            _repo.SaveMeasurement(MakeLengthAddEntity());
            _repo.SaveMeasurement(new QuantityMeasurementEntity("1 Kg", "1000 g", "Equality", "Equal", "Weight"));
            _repo.SaveMeasurement(new QuantityMeasurementEntity("1 Litre", "Conversion", "1000 Millilitre", "Volume"));

            var all = _repo.GetAllMeasurements().ToList();
            Assert.That(all.Count, Is.EqualTo(3));
        }

        [Test]
        public void TestGetTotalCount_ReturnsCorrectNumber()
        {
            Assert.That(_repo.GetTotalCount(), Is.EqualTo(0), "Count should be 0 on empty table.");

            _repo.SaveMeasurement(MakeLengthAddEntity());
            _repo.SaveMeasurement(MakeLengthAddEntity());
            _repo.SaveMeasurement(MakeLengthAddEntity());

            Assert.That(_repo.GetTotalCount(), Is.EqualTo(3));
        }

        [Test]
        public void TestSqlInjectionPrevention_TreatedAsLiteral()
        {
            // This malicious string would break non-parameterised queries —
            // with @param binding it is stored/retrieved as a literal value.
            string injection = "'; DROP TABLE QuantityMeasurements; --";

            _repo.SaveMeasurement(
                new QuantityMeasurementEntity(injection, "N/A", "Addition", "N/A", "Length"));

            var all = _repo.GetAllMeasurements().ToList();
            Assert.That(all.Count, Is.EqualTo(1), "Table must still exist and contain 1 row.");
            Assert.That(all[0].FirstOperand, Is.EqualTo(injection),
                "The injection string should be stored verbatim, not executed.");
        }

        [Test]
        public void TestErrorEntity_RoundTrip()
        {
            var errEntity = new QuantityMeasurementEntity(
                "1 Feet", "1 Kg", "Equality", "Cross-type comparison not allowed", true, "N/A");

            _repo.SaveMeasurement(errEntity);

            var retrieved = _repo.GetAllMeasurements().First();
            Assert.That(retrieved.HasError, Is.True);
            Assert.That(retrieved.FinalResult, Is.EqualTo("Error"));
            Assert.That(retrieved.ErrorMessage, Does.Contain("Cross-type"));
        }
    }
}
