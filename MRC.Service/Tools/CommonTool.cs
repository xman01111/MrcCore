﻿using MRC.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MRC.Service.Tools
{
    public static class CommonTool
    {
        /// <summary>
        /// 判断一个字符串是否全有数字组成
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsDigit(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (!Char.IsDigit(input[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 判断字符串中是否包含字母
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool HasLetter(string input)
        {
            if (input == null)
                throw new ArgumentNullException();
            if (input == string.Empty)
            {
                return false;
            }

            Match match = Regex.Match(input, @"[A-Za-z]");
            if (match != null && match.Success == true)
                return true;

            return false;
        }
        /// <summary>
        /// 判断用户名中是否包含非法字符。如果包含非法字符，则返回false。
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="illegalChar"></param>
        /// <returns></returns>
        public static bool HasIllegalChar(string accountName, out string illegalChar)
        {
            /*
             * 
             */

            //[^a-z]

            illegalChar = null;

            if (accountName.IsNullOrEmpty())
                throw new ArgumentException();

            Match match = Regex.Match(accountName, @"[^A-Za-z0-9_\.-]");
            if (match == null || match.Success == false)
                return false;

            illegalChar = match.Value;
            return true;
        }

        /// <summary>
        /// 检查一个用户名是否合法。
        /// </summary>
        /// <param name="accountName"></param>
        public static void EnsureAccountNameLegal(string accountName, int minLength = 6, int maxLength = 18)
        {
            if (accountName == null)
                throw new ArgumentNullException();

            if (accountName.Length < minLength || accountName.Length > maxLength)
                throw new InvalidInputException("用户名长度必须为 {0}-{1} 位".ToFormat(minLength, maxLength));

            if (!CommonTool.HasLetter(accountName))
                throw new InvalidInputException("用户名至少包含一个字母");
            string illegalChar;
            if (CommonTool.HasIllegalChar(accountName, out illegalChar))
                throw new InvalidInputException("用户名包含非法字符[{0}]".ToFormat(illegalChar));
        }

        public static bool IsMobilePhone(string input)
        {
            Regex regex = new Regex("^1[34578]\\d{9}$");
            return regex.IsMatch(input);
        }
        /// <summary>
        /// 判断一个字符串是否是邮箱地址
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsEmail(string input)
        {
            // /^[0-9A-Za-z][\.-_0-9A-Za-z]*@[0-9A-Za-z]+(\.[0-9A-Za-z]+)+$/
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            Match match = Regex.Match(input, @"^[0-9A-Za-z][\.-_0-9A-Za-z]*@[0-9A-Za-z]+(\.[0-9A-Za-z]+)+$");
            if (match != null && match.Success == true)
                return true;

            return false;
        }
    }
}
