using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

class Image
{
    public uint width = 0;
    public uint height = 0;
    public string compression = null;
    public string textureGroup = null;
    public bool preserveArtistData = false;
    public string importFile = null;
    public string format = null;
    public int xenonDownscaleBias = 0;
    public List<byte[]> mipmaps = new List<byte[]>();
}

static class Program
{
    // Читает число, которое закодировано последовательностью переменной длины.
    // Первый бит первого байта - это знак числа (0 - это +).
    // Второй бит первого байта - есть ли второй байт (1 - есть).
    // Остальные 6 битов первого байта - значащие.
    // Первый бит второго и всех других байтов - есть ли следующий байт (1 - есть).
    // остальные 7 битов второго и всех других байтов - значащие.
    public static int ReadEncodedInt(this BinaryReader reader)
    {
        int b = reader.ReadByte(); // Читаем первый байт
        int result = b & 0b0011_1111; // Значащие биты первого байта
        int sign = (b & 0b1000_0000) != 0 ? -1 : 1; // Знак числа
        bool nextByteRequired = (b & 0b0100_0000) != 0; // Есть ли второй байт

        // Читаем второй и последующие байты
        int shift = 6; // Куда будет вставлен очередной фрагмент числа
        while (nextByteRequired)
        {
            b = reader.ReadByte();
            nextByteRequired = (b & 0b0100_0000) != 0; // Есть ли следующий байт
            int part = b & 0b0111_1111; // Значащие биты
            result |= part << shift; // Добавляем значащие биты к результату
            shift += 7;
        }

        return result * sign;
    }

    public static string ReadEncodedString(this BinaryReader reader)
    {
        // В начале строки - количество символов
        int numChars = reader.ReadEncodedInt();

        // Число символов всегда отрицательно (это означает, что строка в однобайтовой кодировке)
        if (numChars > 0)
            throw new Exception("numChars > 0");

        byte[] bytes = reader.ReadBytes(-numChars);
        return Encoding.ASCII.GetString(bytes);
    }

    // Имя файла передается только для сообщения об ошибке
    static Image Parse(BinaryReader reader, string fileName)
    {
        // Сигнатура файла (4 байта)
        uint signature = reader.ReadUInt32();
        if (signature != 0x57325243) // Файл должен начинаться с CR2W
            throw new Exception("signature != 0x57325243");

        // Версия формата (4 байта)
        uint version = reader.ReadUInt32();
        if (version != 0x73) // Поддерживается только версия 115
            throw new Exception("version != 0x73");

        // Что-то (4 байта)
        uint something0 = reader.ReadUInt32();
        if (something0 != 0) // Всегда ноль
            throw new Exception("something0 != 0");

        // Позиция, с которой начинается список строк (4 байта)
        uint stringsOffset = reader.ReadUInt32();
        if (stringsOffset != 0x2C) // Всегда 44
            throw new Exception("stringsOffset != 0x2C");

        // Количество строк в списке (4 байта)
        uint numStrings = reader.ReadUInt32();
        if (numStrings == 0) // Всегда больше нуля
            throw new Exception("numStrings == 0");

        // Позиция, с которой начинается заголовок объекта (в данном случае CBitmapTexture) (4 байта)
        uint objectHeaderOffset = reader.ReadUInt32();

        // Что-то (4 байта)
        uint something1 = reader.ReadUInt32();
        if (something1 != 1) // Всегда 1
            throw new Exception("something1 != 1");

        // Что-то (4 байта)
        uint something2 = reader.ReadUInt32();
        if (something2 != objectHeaderOffset) // Всегда == objectHeaderOffset
            throw new Exception("something2 != objectHeaderOffset");

        // Что-то (4 байта)
        uint something3 = reader.ReadUInt32();
        if (something3 != 0) // Всегда ноль
            throw new Exception("something3 != 0");

        // Что-то (4 байта)
        uint something4 = reader.ReadUInt32();
        if (something4 != objectHeaderOffset) // Всегда == objectHeaderOffset
            throw new Exception("something4 != objectHeaderOffset");

        // Что-то (4 байта)
        uint something5 = reader.ReadUInt32();
        if (something5 != 1) // Всегда 1
            throw new Exception("something5 != 1");

        // При правильной обработке файла мы должны оказаться в начале списка строк
        if (reader.BaseStream.Position != stringsOffset)
            throw new Exception("reader.BaseStream.Position != stringsOffset");

        // Читаем список строк (в этом списке индексация начинается с 1)
        List<string> strings = new List<string>();
        for (int i = 0; i < numStrings; i++)
            strings.Add(reader.ReadEncodedString());

        // Проверяем, что первая строка - CBitmapTexture (то есть мы парсим именно .xbm)
        if (strings[0] != "CBitmapTexture")
            throw new Exception("strings[1] != \"CBitmapTexture\"");

        // При правильной обработке файла мы должны оказаться в начале заголовка объекта CBitmapTexture
        if (reader.BaseStream.Position != objectHeaderOffset)
            throw new Exception("reader.BaseStream.Position != objectHeaderOffset");

        // ID типа объекта (2 байта)
        ushort something6 = reader.ReadUInt16();
        if (something6 != 1) // Всегда 1, то есть CBitmapTexture
            throw new Exception("something6 != 1");

        // Что-то (4 байта)
        uint something7 = reader.ReadUInt32();
        if (something7 != 0) // Всегда ноль
            throw new Exception("something7 != 0");

        // Размер объекта (4 байта)
        uint objectSize = reader.ReadUInt32();

        // Позиция, с которой начинается сам объект (т.е первое поле объекта) (4 байта)
        uint objectOffset = reader.ReadUInt32();

        // Что-то (4 байта)
        uint something8 = reader.ReadUInt32();
        if (something8 != 0) // Всегда ноль
            throw new Exception("something8 != 0");

        // Что-то (4 байта)
        uint something9 = reader.ReadUInt32();
        if (something9 != 0) // Всегда ноль
            throw new Exception("something9 != 0");

        // Что-то (4 байта)
        byte something10 = reader.ReadByte();
        if (something10 != 0x80) // Всегда 128
            throw new Exception("something10 != 0x80");

        // При правильной обработке файла мы должны оказаться в начале объекта CBitmapTexture
        if (reader.BaseStream.Position != objectOffset)
            throw new Exception("reader.BaseStream.Position != objectOffset");

        // Проверяем, что объект занимает все место до конца файла
        if (reader.BaseStream.Length != objectOffset + objectSize)
            throw new Exception("reader.BaseStream.Length != objectOffset + objectSize");

        // Считываем поля объекта CBitmapTexture

        Image result = new Image();

        while (true)
        {
            ushort fieldNameID = reader.ReadUInt16(); // ID имени поля (4 байта)

            if (fieldNameID == 0)
                break; // Полей больше нет

            string fieldName = strings[fieldNameID - 1]; // - 1 так как индексация начинается с 1
            ushort fieldTypeID = reader.ReadUInt16(); // ID типа поля (4 байата)
            string fieldType = strings[fieldTypeID - 1];

            // Что-то (2 байта)
            ushort something11 = reader.ReadUInt16();
            if (something11 != 0xFFFF) // Всегда 0xFFFF
                throw new Exception("something11 != 0xFFFF");

            long fieldStart = reader.BaseStream.Position;

            // Размер поля (4 байта)
            uint fieldSize = reader.ReadUInt32();

            if (fieldName == "width" && fieldType == "Uint")
            {
                result.width = reader.ReadUInt32();
            }
            else if (fieldName == "height" && fieldType == "Uint")
            {
                result.height = reader.ReadUInt32();
            }
            else if (fieldName == "compression" && fieldType == "ETextureCompression")
            {
                ushort compressionID = reader.ReadUInt16();
                result.compression = strings[compressionID - 1];

                if (result.compression != "TCM_DXTNoAlpha" && result.compression != "TCM_Normals" &&
                    result.compression != "TCM_DXTAlpha" && result.compression != "TCM_NormalsHigh")
                    throw new Exception("Incorrect compression");
            }
            else if (fieldName == "textureGroup" && fieldType == "CName")
            {
                ushort textureGroupID = reader.ReadUInt16();
                result.textureGroup = strings[textureGroupID - 1];
            }
            else if (fieldName == "preserveArtistData" && fieldType == "Bool")
            {
                result.preserveArtistData = reader.ReadBoolean(); // 1 байт
            }
            else if (fieldName == "importFile" && fieldType == "String")
            {
                result.importFile = reader.ReadEncodedString();
            }
            else if (fieldName == "format" && fieldType == "ETextureRawFormat")
            {
                ushort formatID = reader.ReadUInt16();
                result.format = strings[formatID - 1];

                if (result.format != "TRF_Grayscale")
                    throw new Exception("result.format != \"TRF_Grayscale\"");
            }
            else if (fieldName == "xenonDownscaleBias" && fieldType == "Int")
            {
                result.xenonDownscaleBias = reader.ReadInt32();
            }
            else
                throw new Exception("Unknown field");

            // Проверяем, что считали поле правильно
            if (reader.BaseStream.Position != fieldStart + fieldSize)
                throw new Exception("reader.BaseStream.Position != fieldStart + fieldSize");
        }

        // Что-то (1 байт)
        byte something12 = reader.ReadByte();
        if (something12 != 0) // Всегда 0
            throw new Exception("something12 != 0");

        // Что-то (4 байта)
        uint something13 = reader.ReadUInt32();
        if (something13 != 0) // Всегда 0
            throw new Exception("omething13 != 0");

        // Число MIP-уровней
        uint mipmapCount = reader.ReadUInt32();

        if (mipmapCount == 0)
            throw new Exception("mipmapCount == 0");

        for (uint i = 0; i < mipmapCount; i++)
        {
            uint mipWidth = reader.ReadUInt32();
            uint mipHeight = reader.ReadUInt32();

            uint something14 = reader.ReadUInt32();

            int size = reader.ReadInt32();
            byte[] data = reader.ReadBytes(size);

            result.mipmaps.Add(data);
        }

        // Проверяем, правильный ли конец файла. Несколько файлов в игре битые, но их все еще можно распаковать

        // Что-то (4 байта)
        uint something15 = reader.ReadUInt32();
        if (something15 != 0) // Всегда 0
            Console.WriteLine("\n" + fileName + " | something15 != 0");

        // Что-то (1 байт)
        byte something16 = reader.ReadByte();
        if (something16 != 0) // Всегда 0
            Console.WriteLine("\n" + fileName + " | something16 != 0");

        // Проверяем, что дошли до конца файла
        if (reader.BaseStream.Position != reader.BaseStream.Length)
            Console.WriteLine("\n" + fileName + " | reader.BaseStream.Position != reader.BaseStream.Length");

        return result;
    }

    // TRF_Grayscale нигде не используется
    static void Save(Image image, BinaryWriter writer)
    {
        // Начинаем запись в DDS
        // https://docs.microsoft.com/en-us/windows/desktop/direct3ddds/dx-graphics-dds-pguide

        writer.Write(0x20534444u); // Сигнатура файла "DDS "

        // Пишем DDS_HEADER

        writer.Write(124u); // dwSize

        uint ddsFlags = 0x1u; // DDSD_CAPS
        ddsFlags |= 0x2u; // DDSD_HEIGHT
        ddsFlags |= 0x4u; // DDSD_WIDTH
        ddsFlags |= 0x1000u; // DDSD_PIXELFORMAT
        if (image.mipmaps.Count > 0)
            ddsFlags |= 0x20000u; // DDSD_MIPMAPCOUNT
        writer.Write(ddsFlags); // dwFlags

        writer.Write(image.height); // dwHeight
        writer.Write(image.width); // dwWidth
        writer.Write(0u); // dwPitchOrLinearSize
        writer.Write(0u); // dwDepth
        writer.Write(image.mipmaps.Count); // dwMipMapCount

        for (int i = 0; i < 11; i++)
            writer.Write(0u); // dwReserved1[11]

        // Пишем DDS_PIXELFORMAT в DDS_HEADER

        writer.Write(32u); // dwSize

        uint pfFlags = 0x0u;
        if (image.compression == "TCM_DXTAlpha")
            pfFlags |= 0x1u; // DDPF_ALPHAPIXELS
        if (image.compression != null)
            pfFlags |= 0x4u; // DDPF_FOURCC
        else
            pfFlags |= 0x40u;// DDPF_RGB
        writer.Write(pfFlags); // dwFlags

        // dwFourCC
        if (image.compression == "TCM_DXTNoAlpha" || image.compression == "TCM_Normals")
            writer.Write(0x31545844u); // "DXT1"
        else if (image.compression == "TCM_DXTAlpha" || image.compression == "TCM_NormalsHigh")
            writer.Write(0x35545844u); // "DXT5"
        else if (image.compression == null)
            writer.Write(0u); // Нет сжатия
        else
            throw new Exception("Unknown compression");

        if (image.compression != null)
        {
            writer.Write(0u); // dwRGBBitCount
            writer.Write(0u); // dwRBitMask
            writer.Write(0u); // dwGBitMask
            writer.Write(0u); // dwBBitMask
            writer.Write(0u); // dwABitMask
        }
        else
        {
            writer.Write(32u); // dwRGBBitCount
            writer.Write(0x00FF0000u); // dwRBitMask
            writer.Write(0x0000FF00u); // dwGBitMask
            writer.Write(0x000000FFu); // dwBBitMask
            writer.Write(0xFF000000u); // dwABitMask
        }

        // Завершили писать DDS_PIXELFORMAT, продолжаем писать DDS_HEADER

        uint caps = 0x1000; // DDSCAPS_TEXTURE
        if (image.mipmaps.Count > 0)
        {
            caps |= 0x400000; // DDSCAPS_MIPMAP
            caps |= 0x8; // DDSCAPS_COMPLEX
        }
        writer.Write(caps); // dwCaps

        writer.Write(0u); // dwCaps2
        writer.Write(0u); // dwCaps3
        writer.Write(0u); // dwCaps4
        writer.Write(0u); // dwReserved2

        // Завершили заголовок

        foreach (byte[] data in image.mipmaps)
            writer.Write(data);
    }


    static void PrintUsage()
    {
        Console.WriteLine("Usage: W2XBM.exe \"input dir or file\" \"output dir\"");
    }

    // В отличие от Path.GetFileNameWithoutExtension() работает с путями, в которых используется / вместо \
    static string RemoveExtension(string path)
    {
        int index = path.LastIndexOf('.');

        if (index == -1)
            return path;

        return path.Substring(0, index);
    }

    static string StandardizePath(string path)
    {
        path = Path.GetFullPath(path).Replace('\\', '/');

        if (path[path.Length - 1] == '/')
            path = path.Substring(path.Length - 1);

        return path;
    }

    static string GetOnlyFileNameWithoutExtension(string path)
    {
        FileInfo info = new FileInfo(path);
        return RemoveExtension(info.Name);
    }

    static void ProcessFile(string inputFile, string outputDir)
    {
        try
        {
            string outputFile = outputDir + "/" + GetOnlyFileNameWithoutExtension(inputFile) + ".dds";

            using (BinaryReader reader = new BinaryReader(File.Open(inputFile, FileMode.Open)))
            {
                Image image = Parse(reader, inputFile);

                using (BinaryWriter writer = new BinaryWriter(File.Open(outputFile, FileMode.Create)))
                {
                    Save(image, writer);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(inputFile);
            Console.WriteLine(e.Message);
        }
    }

    static void ProcessDir(string inputDir, string outputDir)
    {
        foreach (string fileName in Directory.GetFiles(inputDir, "*.xbm", SearchOption.AllDirectories))
        {
            Console.Write(".");

            try
            {
                string inputFile = StandardizePath(fileName);

                if (!inputFile.StartsWith(inputDir)) // По идее такое исключение никогда не возникнет
                    throw new Exception("!inputFile.StartsWith(inputDir)");

                string outputFile = RemoveExtension(outputDir + inputFile.Substring(inputDir.Length)) + ".dds";

                using (BinaryReader reader = new BinaryReader(File.Open(inputFile, FileMode.Open)))
                {
                    Image image = Parse(reader, inputFile);

                    Directory.CreateDirectory(Path.GetDirectoryName(outputFile));

                    using (BinaryWriter writer = new BinaryWriter(File.Open(outputFile, FileMode.Create)))
                    {
                        Save(image, writer);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine(fileName);
                Console.WriteLine(e.Message);
            }

        }
    }

    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            PrintUsage();
            return;
        }

        string input = StandardizePath(args[0]);
        if (!File.Exists(input) && !Directory.Exists(input))
        {
            PrintUsage();
            return;
        }

        string output = StandardizePath(args[1]);
        try
        {
            Directory.CreateDirectory(output);
        }
        catch
        {
            PrintUsage();
            return;
        }

        if (File.Exists(input))
            ProcessFile(input, output);
        else
            ProcessDir(input, output);

        return;
    }
}   
