using Npgsql;

namespace SupabaseInit;

/// <summary>
/// Supabase database initialization tool
/// Creates tables and inserts sample data
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        // Supabase connection string
        // Format: postgresql://postgres.[project-ref]:[password]@aws-0-ap-northeast-2.pooler.supabase.com:6543/postgres
        Console.WriteLine("=== Supabase Database Initialization ===");
        Console.WriteLine();
        
        Console.Write("Enter Supabase Database Password: ");
        var password = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Error: Password cannot be empty");
            return;
        }

        var connectionString = $"Host=aws-0-ap-northeast-2.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.lmybwtyciqiaasydhttk;Password={password};SSL Mode=Require;Trust Server Certificate=true";

        try
        {
            await using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();
            Console.WriteLine("✅ Connected to Supabase!");
            Console.WriteLine();

            // Step 1: Create persons table
            Console.WriteLine("[1/3] Creating persons table...");
            await CreatePersonsTableAsync(conn);
            Console.WriteLine("✅ Table created");
            Console.WriteLine();

            // Step 2: Insert users
            Console.WriteLine("[2/3] Inserting 10 users...");
            await InsertUsersAsync(conn);
            Console.WriteLine("✅ Users inserted");
            Console.WriteLine();

            // Step 3: Insert assistants
            Console.WriteLine("[3/3] Inserting 20 assistants...");
            await InsertAssistantsAsync(conn);
            Console.WriteLine("✅ Assistants inserted");
            Console.WriteLine();

            // Verify data
            Console.WriteLine("=== Verification ===");
            await VerifyDataAsync(conn);
            
            Console.WriteLine();
            Console.WriteLine("🎉 Database initialization complete!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }

    static async Task CreatePersonsTableAsync(NpgsqlConnection conn)
    {
        var sql = @"
-- Enable PostGIS extension
CREATE EXTENSION IF NOT EXISTS postgis;

-- Drop existing table if exists
DROP TABLE IF EXISTS persons CASCADE;

-- Create persons table
CREATE TABLE persons (
  id SERIAL PRIMARY KEY,
  
  -- Basic info
  name VARCHAR(100) NOT NULL,
  type VARCHAR(20) NOT NULL CHECK (type IN ('user', 'assistant')),
  email VARCHAR(255) UNIQUE NOT NULL,
  phone VARCHAR(20),
  
  -- Address & Location
  address TEXT NOT NULL,
  latitude DOUBLE PRECISION NOT NULL,
  longitude DOUBLE PRECISION NOT NULL,
  
  -- Profile
  gender VARCHAR(10) CHECK (gender IN ('male', 'female', 'other', NULL)),
  has_vehicle BOOLEAN DEFAULT false,
  available_time_slots VARCHAR(50),
  experience_years INTEGER,
  
  -- Metadata
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Create indexes
CREATE INDEX idx_persons_type ON persons (type);
CREATE INDEX idx_persons_email ON persons (email);
";

        await using var cmd = new NpgsqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    static async Task InsertUsersAsync(NpgsqlConnection conn)
    {
        var sql = @"
INSERT INTO persons (id, name, type, email, phone, address, latitude, longitude, gender, has_vehicle, available_time_slots, experience_years)
VALUES
(1, '김영희', 'user', 'user1@example.com', '010-1234-5001', '서울 도봉구 도봉동 123', 37.6738, 127.0501, 'female', false, NULL, NULL),
(2, '이철수', 'user', 'user2@example.com', '010-1234-5002', '서울 도봉구 쌍문동 234', 37.6608, 127.0531, 'male', false, NULL, NULL),
(3, '박지영', 'user', 'user3@example.com', '010-1234-5003', '서울 도봉구 방학동 345', 37.6796, 127.0431, 'female', false, NULL, NULL),
(4, '최민수', 'user', 'user4@example.com', '010-1234-5004', '서울 도봉구 창동 456', 37.6538, 127.0391, 'male', false, NULL, NULL),
(5, '정수진', 'user', 'user5@example.com', '010-1234-5005', '서울 강북구 미아동 567', 37.6850, 127.0571, 'female', false, NULL, NULL),
(6, '한동훈', 'user', 'user6@example.com', '010-1234-5006', '서울 강북구 번동 678', 37.6488, 127.0591, 'male', false, NULL, NULL),
(7, '윤서연', 'user', 'user7@example.com', '010-1234-5007', '서울 노원구 상계동 789', 37.6938, 127.0321, 'female', false, NULL, NULL),
(8, '강태오', 'user', 'user8@example.com', '010-1234-5008', '서울 노원구 중계동 890', 37.6368, 127.0291, 'male', false, NULL, NULL),
(9, '임은지', 'user', 'user9@example.com', '010-1234-5009', '서울 성북구 장위동 901', 37.7008, 127.0671, 'female', false, NULL, NULL),
(10, '조현우', 'user', 'user10@example.com', '010-1234-5010', '서울 성북구 석관동 012', 37.6338, 127.0691, 'male', false, NULL, NULL);
";

        await using var cmd = new NpgsqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    static async Task InsertAssistantsAsync(NpgsqlConnection conn)
    {
        var sql = @"
INSERT INTO persons (id, name, type, email, phone, address, latitude, longitude, gender, has_vehicle, available_time_slots, experience_years)
VALUES
-- 반경 0-1km
(11, '김지원', 'assistant', 'assistant1@example.com', '010-2001-0001', '서울 도봉구 도봉동 111-1', 37.6718, 127.0491, 'female', true, 'weekday_am', 5),
(12, '이민호', 'assistant', 'assistant2@example.com', '010-2001-0002', '서울 도봉구 도봉동 111-2', 37.6648, 127.0501, 'male', true, 'weekday_pm', 3),
(13, '박서현', 'assistant', 'assistant3@example.com', '010-2001-0003', '서울 도봉구 쌍문동 222-1', 37.6733, 127.0451, 'female', false, 'weekend', 2),
(14, '최우진', 'assistant', 'assistant4@example.com', '010-2001-0004', '서울 도봉구 쌍문동 222-2', 37.6628, 127.0431, 'male', true, 'weekday_am', 4),
(15, '정다은', 'assistant', 'assistant5@example.com', '010-2001-0005', '서울 도봉구 방학동 333-1', 37.6751, 127.0521, 'female', true, 'weekday_pm', 6),
-- 반경 1-2km
(16, '한지훈', 'assistant', 'assistant6@example.com', '010-2001-0006', '서울 도봉구 방학동 333-2', 37.6588, 127.0551, 'male', false, 'weekend', 1),
(17, '윤채원', 'assistant', 'assistant7@example.com', '010-2001-0007', '서울 도봉구 창동 444-1', 37.6796, 127.0381, 'female', true, 'weekday_am', 5),
(18, '강민석', 'assistant', 'assistant8@example.com', '010-2001-0008', '서울 도봉구 창동 444-2', 37.6558, 127.0371, 'male', true, 'weekday_pm', 4),
(19, '임소라', 'assistant', 'assistant9@example.com', '010-2001-0009', '서울 강북구 미아동 555-1', 37.6823, 127.0581, 'female', false, 'weekend', 7),
(20, '조태호', 'assistant', 'assistant10@example.com', '010-2001-0010', '서울 강북구 미아동 555-2', 37.6528, 127.0591, 'male', true, 'weekday_am', 3),
-- 반경 2-3km
(21, '신예린', 'assistant', 'assistant11@example.com', '010-2001-0011', '서울 강북구 번동 666-1', 37.6868, 127.0331, 'female', true, 'weekday_pm', 8),
(22, '배준영', 'assistant', 'assistant12@example.com', '010-2001-0012', '서울 강북구 번동 666-2', 37.6478, 127.0321, 'male', false, 'weekend', 6),
(23, '서유진', 'assistant', 'assistant13@example.com', '010-2001-0013', '서울 노원구 상계동 777-1', 37.6919, 127.0631, 'female', true, 'weekday_am', 5),
(24, '오현수', 'assistant', 'assistant14@example.com', '010-2001-0014', '서울 노원구 상계동 777-2', 37.6448, 127.0641, 'male', true, 'weekday_pm', 2),
(25, '권나영', 'assistant', 'assistant15@example.com', '010-2001-0015', '서울 노원구 중계동 888-1', 37.6948, 127.0291, 'female', false, 'weekend', 9),
-- 반경 3-4km
(26, '송재민', 'assistant', 'assistant16@example.com', '010-2001-0016', '서울 노원구 중계동 888-2', 37.6408, 127.0271, 'male', true, 'weekday_am', 4),
(27, '홍수아', 'assistant', 'assistant17@example.com', '010-2001-0017', '서울 성북구 장위동 999-1', 37.6988, 127.0681, 'female', true, 'weekday_pm', 10),
(28, '노지훈', 'assistant', 'assistant18@example.com', '010-2001-0018', '서울 성북구 장위동 999-2', 37.6368, 127.0701, 'male', false, 'weekend', 7),
(29, '황지원', 'assistant', 'assistant19@example.com', '010-2001-0019', '서울 성북구 석관동 000-1', 37.7028, 127.0231, 'female', true, 'weekday_am', 3),
(30, '안태양', 'assistant', 'assistant20@example.com', '010-2001-0020', '서울 성북구 석관동 000-2', 37.6328, 127.0211, 'male', true, 'weekday_pm', 12);

-- Reset sequence
SELECT setval('persons_id_seq', 30, true);
";

        await using var cmd = new NpgsqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    static async Task VerifyDataAsync(NpgsqlConnection conn)
    {
        var sql = "SELECT type, COUNT(*) as count FROM persons GROUP BY type ORDER BY type";
        
        await using var cmd = new NpgsqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            var type = reader.GetString(0);
            var count = reader.GetInt64(1);
            Console.WriteLine($"  {type}: {count} records");
        }
    }
}



