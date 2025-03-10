// See https://aka.ms/new-console-template for more information
using System.IO;
Console.WriteLine("=== 龙魂旅人Bundle文件处理 ===");

// 交互式输入路径1
Console.Write("请输入包含原始文件的目录路径：");
string inputDir = Console.ReadLine()?.Trim('"').Trim();

Console.Write("请输入输出目录路径：");
string outputDir = Console.ReadLine()?.Trim('"').Trim();

// 验证输入路径
if (!Directory.Exists(inputDir))
{
    Console.WriteLine("原始文件目录不存在！");
    Console.ReadKey();
    return;
}

try
{
    // 创建输出目录（如果不存在）
    Directory.CreateDirectory(outputDir);

    // UnityFS的十六进制模式（ASCII值）
    byte[] pattern = { 0x55, 0x6E, 0x69, 0x74, 0x79, 0x46, 0x53 };
    int processedCount = 0;

    foreach (string filePath in Directory.GetFiles(inputDir))
    {
        try
        {
            Console.WriteLine($"正在处理: {Path.GetFileName(filePath)}");

            // 读取文件内容
            byte[] fileData = File.ReadAllBytes(filePath);

            // 查找第二个UnityFS出现位置
            //int cutPosition = FindNthOccurrence(fileData, pattern, 2);
            int lastUnityFS = FindLastOccurrence(fileData, pattern);

            if (lastUnityFS == -1)
            {
                Console.WriteLine($"UnityFS文件头数量异常，已跳过: {filePath}");
                continue;
            }
         
            byte[] newFileData = new byte[fileData.Length - lastUnityFS];
            Array.Copy(fileData, lastUnityFS, newFileData, 0, newFileData.Length);

            string outputPath = Path.Combine(outputDir, Path.GetFileName(filePath));
            File.WriteAllBytes(outputPath, newFileData);

            processedCount++;
            Console.WriteLine($"成功处理，已保存到: {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"处理文件时发生错误: {ex.Message}");
        }
    }
    Console.WriteLine($"\n处理完成！共处理 {processedCount} 个文件");

}
catch (Exception ex)
{
    Console.WriteLine($"发生错误：{ex.Message}");
}

Console.WriteLine("\n按任意键退出...");
Console.ReadKey();

//static int FindNthOccurrence(byte[] data, byte[] pattern, int n)
//{
//    int matchCount = 0;
//    int patternLength = pattern.Length;
//    int maxSearchIndex = data.Length - patternLength;

//    for (int i = 0; i <= maxSearchIndex; i++)
//    {
//        bool isMatch = true;

//        for (int j = 0; j < patternLength; j++)
//        {
//            if (data[i + j] != pattern[j])
//            {
//                isMatch = false;
//                break;
//            }
//        }

//        if (isMatch)
//        {
//            matchCount++;
//            if (matchCount == n)
//            {
//                return i;
//            }
//        }
//    }

//    return -1;
//}

static int FindLastOccurrence(byte[] data, byte[] pattern)
{
    int lastPos = -1;
    for (int i = 0; i <= data.Length - pattern.Length; i++)
    {
        bool match = true;
        for (int j = 0; j < pattern.Length; j++)
        {
            if (data[i + j] != pattern[j])
            {
                match = false;
                break;
            }
        }
        if (match) lastPos = i;
    }
    return lastPos;
}
