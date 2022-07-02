// Decompresses special Xamarin .dll files from the Android APKs
// Written in C# as its easier than python if you are developing in c#
// Based on this Great work https://github.com/x41sec/tools/blob/master/Mobile/Xamarin/Xamarin_XALZ_decompress.py all credit to them

// Takes a filename as input and second filename as output.

using System.Text;
using K4os.Compression.LZ4;

//Special Header to look for when decoding dll
var headerAsBytes = Encoding.ASCII.GetBytes("XALZ");

if (args.Length != 2)
{
    Console.WriteLine("Takes a filename as input and second filename as output.");
    return -1;
}
    

var inputFile = args[0];
var outputFile = args[1];
Console.WriteLine($"Input File: {inputFile}, Output File: {outputFile}");

using var binaryReader = new BinaryReader(File.Open(inputFile, FileMode.Open));
binaryReader.BaseStream.Position = 0;

//Check header for a match to file
var headerBytes = binaryReader.ReadBytes(4);
if (!ByteArrayCompare(headerAsBytes, headerBytes))
{
    Console.WriteLine("Input file does not match expected header");
    return -1;
}

//Read header
var headerIndex = binaryReader.ReadBytes(4);

//Struct in the format of little endian unsigned int for the uncompressed file length
var structBytes = binaryReader.ReadBytes(4);
var uncompressedLength = BitConverter.ToUInt32(structBytes);

//Get compressed data
var compressedData = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length - 12);
    
//decompress
var output = new Span<byte>(new byte[uncompressedLength]);
LZ4Codec.Decode(compressedData, output);


File.WriteAllBytes(outputFile, output.ToArray());
Console.WriteLine("Result written to file enjoy");

return 0;

static bool ByteArrayCompare(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2)
{
    return a1.SequenceEqual(a2);
}