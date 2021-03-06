﻿using System.Collections.Generic;

namespace VerteX.Lexing
{
    /// <summary>
    /// Представляет удобную работу со списком токенов.
    /// </summary>
    public class TokenList : List<Token>
    {
        /// <summary>
        /// Возвращает строку валидного кода.
        /// </summary>
        public override string ToString()
        {
            string tokensString = "";
            foreach (Token token in this)
            {
                tokensString += token.ToString();
            }
            return tokensString;
        }

        /// <summary>
        /// Возвращает индекс первого токена с данным содержанием.
        /// </summary>
        public int IndexOf(string value)
        {
            for (int index = 0; index < Count; index++)
            {
                if (this[index].value == value) return index;
            }
            return -1;
        }

        /// <summary>
        /// Возвращает индекс первого токена с данным типом.
        /// </summary>
        public int IndexOf(KeywordType type)
        {
            for (int i = 0; i < Count; i++)
            {
                Token token = this[i];
                if (Keywords.GetKeywordType(token) == type) return i;
            }
            return -1;
        }

        /// <summary>
        /// Возвращает строку, представляющую структуру списка, удобную для отладки.
        /// </summary>
        public string ToDebug()
        {
            string str = "";
            foreach (Token token in this)
            {
                str += $"    [{token.type}]: {token.value}\n";
            }
            return str;
        }

        /// <summary>
        /// Возвращает токен по данному индексу, если такового нет, то возвращает токен неопределённого типа.
        /// </summary>
        public Token Get(int index)
        {
            try
            {
                return this[index];
            }
            catch
            {
                return new Token(TokenType.Undefined, "");
            }
        }

        /// <summary>
        /// Получает срез элементов от начального индекса до конечного не включительно.
        /// </summary>
        /// <param name="firstIndex">Начальный индекс.</param>
        /// <param name="lastIndex">Конечный индекс.</param>
        public new TokenList GetRange(int firstIndex, int lastIndex)
        {
            TokenList newList = new TokenList();
            newList.AddRange(base.GetRange(firstIndex, lastIndex - firstIndex));

            return newList;
        }

        /// <summary>
        /// Получает срез элементов от начального индекса до конца списка.
        /// </summary>
        /// <param name="firstIndex">Начальный индекс.</param>
        public TokenList GetRange(int firstIndex)
        {
            TokenList newList = new TokenList();
            newList.AddRange(base.GetRange(firstIndex, Count - (firstIndex)));

            return newList;
        }

        /// <summary>
        /// Формирует и добавляет новый токен в список.
        /// </summary>
        /// <param name="type">Тип токена.</param>
        /// <param name="value">Его содержимое.</param>
        public void Add(TokenType type, string value)
        {
            Add(new Token(type, value));
        }

        /// <summary>
        /// Формирует и добавляет новый токен в список.
        /// </summary>
        /// <param name="type">Тип токена.</param>
        /// <param name="ch">Его содержимое.</param>
        public void Add(TokenType type, char ch)
        {
            Add(type, ch.ToString());
        }

        /// <summary>
        /// Удаляет все токены в списке, содержащие указанное содержимое.
        /// </summary>
        public void DeleteAll(string value)
        {
            TokenList newList = new TokenList();

            foreach (Token token in this)
            {
                if (token.value != value)
                {
                    newList.Add(token);
                }
            }
            Clear();
            AddRange(newList);
        }

        public new Token this[int index]
        {
            get
            {
                if (index >= 0) return base[index];
                else return base[Count + index];
            }
            set
            {
                if (index >= 0) base[index] = value;
                else base[Count + index] = value;
            }
        }
    }
}
