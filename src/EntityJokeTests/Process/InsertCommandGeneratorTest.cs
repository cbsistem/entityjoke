﻿using EntityJoke.Core;
using EntityJoke.Process.Commands;
using EntityJokeTests.Core;
using NUnit.Framework;
using System;

namespace EntityJokeTests.Process
{
    [TestFixture]
    public class InsertCommandGeneratorTest
    {

        InsertCommandGenerator target;

        [SetUp]
        public void SetUp()
        {
            DictionaryEntitiesMap.INSTANCE.TryAddEntity(typeof(ProdutoTeste));
            DictionaryEntitiesMap.INSTANCE.TryAddEntity(typeof(PrecoProduto));
            DictionaryEntitiesMap.INSTANCE.TryAddEntity(typeof(ComparadorProdutos));
        }

        [Test]
        public void GeraInsertCategoriaTeste()
        {
            CategoriaTeste categoria = new CategoriaTeste();
            categoria.Id = 2;
            categoria.Nome = "Comidas";

            target = new InsertCommandGenerator(categoria);

            string insert = "INSERT INTO categoria_teste (nome) VALUES ('Comidas') RETURNING ID";

            Assert.That(target.Generate(), Is.EqualTo(insert));
        }

        [Test]
        public void GeraInsertProdutoTeste()
        {
            ProdutoTeste produto = new ProdutoTeste();
            produto.Id = 3;
            produto.Nome = "Lasanha";
            produto.Embalagem = "Caixa";
            produto.Marca = "Sadia";
            produto.Quantidade = "650";
            produto.UnidadeMedida = "g";

            produto.CategoriaTeste = new CategoriaTeste();
            produto.CategoriaTeste.Id = 4;
            produto.CategoriaTeste.Nome = "Congelados";

            target = new InsertCommandGenerator(produto);

            string insert = "";
            insert += "INSERT INTO produto_teste (id_categoria_teste, embalagem, marca, nome, quantidade, unidade_medida) VALUES (4, 'Caixa', 'Sadia', 'Lasanha', '650', 'g') RETURNING ID";

            Assert.That(target.Generate(), Is.EqualTo(insert));
        }

        [Test]
        public void GeraInsertProdutoCategoriaNulaTeste()
        {
            ProdutoTeste produto = new ProdutoTeste();
            produto.Id = 3;
            produto.Nome = "Lasanha";
            produto.Embalagem = "Caixa";
            produto.Marca = "Sadia";
            produto.Quantidade = "650";
            produto.UnidadeMedida = "g";

            target = new InsertCommandGenerator(produto);

            string insert = "";
            insert += "INSERT INTO produto_teste (embalagem, marca, nome, quantidade, unidade_medida) VALUES ('Caixa', 'Sadia', 'Lasanha', '650', 'g') RETURNING ID";

            Assert.That(target.Generate(), Is.EqualTo(insert));
        }

        [Test]
        public void GeraInsertPrecoProduto()
        {
            DateTime dataIni = new DateTime(2015, 11, 07);
            DateTime dataFim = new DateTime(2015, 11, 09);

            PrecoProduto precoProduto = new PrecoProduto();
            precoProduto.Id = 10;
            precoProduto.Preco = 20;
            precoProduto.DataInicio = dataIni;
            precoProduto.DataFim = dataFim;

            precoProduto.Produto = new Produto();
            precoProduto.Produto.Id = 4;
            precoProduto.Produto.Nome = "Trigo";

            target = new InsertCommandGenerator(precoProduto);

            string insert = "";
            insert += "INSERT INTO preco_produto (data_fim, data_inicio, preco, id_produto) VALUES ('" + dataFim.GetDateTimeFormats()[54] + "', '" + dataIni.GetDateTimeFormats()[54] + "', 20, 4) RETURNING ID";

            Assert.That(target.Generate(), Is.EqualTo(insert));
        }

        [Test]
        public void GeraInsertComparadorProduto()
        {
            DateTime data = new DateTime(2015, 11, 07);

            ComparadorProdutos comparador = new ComparadorProdutos();
            comparador.Id = 20;
            comparador.DataComparacao = data;

            comparador.ProdutoA = new Produto();
            comparador.ProdutoA.Id = 4;
            comparador.ProdutoA.Nome = "Trigo";

            comparador.ProdutoB = new Produto();
            comparador.ProdutoB.Id = 23;
            comparador.ProdutoB.Nome = "Macarrão";

            target = new InsertCommandGenerator(comparador);

            string insert = "";
            insert += "INSERT INTO comparador_produtos (data_comparacao, id_produto_a, id_produto_b) VALUES ('" + data.GetDateTimeFormats()[54] + "', 4, 23) RETURNING ID";

            Assert.That(target.Generate(), Is.EqualTo(insert));
        }

    }
}