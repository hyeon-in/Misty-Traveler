using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// CSV 파일을 읽어오기 위한 클래스입니다.
/// </summary>
public class CSVReader
{
    // CSV 필드를 나누기 위한 정규 표현식
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";

    // 줄을 나누기 위한 정규 표현식
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

    // 값에서 제거할 문자들의 배열
    static char[] TRIM_CHARS = { '\"' };

    /// <summary>
    /// CSV 파일을 읽어 Dictionary 리스트로 반환하는 정적 메소드 입니다.
    /// </summary>
    /// <param name="file">읽어올 CSV 파일</param>
    /// <returns>CSV 데이터를 담은 Dictionary 리스트</returns>
    public static List<Dictionary<string, object>> Read(string file)
    {
        var list = new List<Dictionary<string, object>>();

        // Resources 폴더에서 CSV 파일 로드
        TextAsset data = Resources.Load(file) as TextAsset;

        // 파일 내용을 줄 단위로 나누기
        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        // 내용의 줄이 한 줄 이하일 경우 빈 리스트를 반환합니다.
        if (lines.Length <= 1) return list;

        // 헤더 추출
        var header = Regex.Split(lines[0], SPLIT_RE);

        // 각 줄의 내용을 리스트에 담습니다.
        for (var i = 1; i < lines.Length; i++)
        {
            // 정규 표현식을 사용하여 값을 나눕니다.
            // 값이 없거나 첫 번째 값이 비어있으면 무시합니다.
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                // 값에서 따옴표를 제거하고 특수 문자를 대체합니다.
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                value = value.Replace("<br>", "\n");
                value = value.Replace("<c>", ",");

                // 값 형변환
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                else if (value.ToLower() == "true")
                {
                    finalvalue = true;
                }
                else if (value.ToLower() == "false")
                {
                    finalvalue = false;
                }

                // 값 추가
                entry[header[j]] = finalvalue;
            }

            // 리스트에 Dictionary 추가
            list.Add(entry);
        }
        return list;
    }
}