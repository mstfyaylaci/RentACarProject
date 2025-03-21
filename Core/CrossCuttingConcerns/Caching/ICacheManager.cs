﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CrossCuttingConcerns.Caching
{
    // herhangi bir teknolojiden bağımsız
    //Key :Business.concrete.carManager.GetAll gibi  value olarak çalışır
    public interface ICacheManager
    {
        T Get<T>(string key);

        object Get(string key);
        void Add(string key, object value, int duration);

        bool IsAdd(string key); // cache de var mı ? yoksa ekle

        void Remove(string key);// cach den sil
        void RemoveByPattern(string pattern); // isminde verilen pattern olanları cache den sil
    }
}
