using System;
using System.Collections.Generic;
using System.Linq;

namespace ContaCorrenteMaximaTECH
{
    public class Vendedor
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }
    public class Produto
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public double PrecoTabela { get; set; }
    }
    public class Pedido
    {
        public int Id { get; set; }
        public int IdVendedor { get; set; }
        public Vendedor Vendedor { get; set; }
        public List<ItemPedido> ItensPedido = new List<ItemPedido>();
        public double TotalPedido => ItensPedido.Sum(p => p.Quantidade * p.PrecoVenda);
        public Pedido(int id, Vendedor vendedor)
        {
            Id = id;
            Vendedor = vendedor;
        }
    }
    public class ItemPedido
    {
        public int IdPedido { get; set; }
        public Pedido Pedido { get; set; }
        public int IdProduto { get; set; }
        public Produto Produto { get; set; }
        public double PrecoVenda { get; set; }
        private double PrecoTabela { get; set; }
        public double PrecoBaseRCA { get; set; }
        public int Seq { get; set; }
        public int Quantidade { get; set; }

        public ItemPedido(Pedido pedido, Produto produto, int seq, int quantidade, double precoVenda, double precoBaseRCA)
        {
            Pedido = pedido;
            Produto = produto;
            Quantidade = quantidade;
            Seq = seq;
            PrecoVenda = precoVenda;
            PrecoTabela = produto.PrecoTabela;
            PrecoBaseRCA = precoBaseRCA;
        }

    }
    public class Log
    {
        public DateTime Data { get; set; }
        public int IdVendedor { get; set; }
        public int IdProduto { get; set; }
        public double Quantidade { get; set; }
        public int Seq { get; set; }
        public double SaldoAtual { get; set; }
        public double LimiteCreditoAtual { get; set; }
        public double LimiteCreditoAnterior { get; set; }
        public double SaldoAnterior { get; set; }
        public string Historico { get; set; }

    }
    public class ContaCorrente
    {
        public ContaCorrente(Vendedor vendedor)
        {
            this.Vendedor = vendedor;
        }
        public Vendedor Vendedor { get; set; }
        private double _limite;
        private double _saldoAtual;
        public IList<Log> Logs = new List<Log>();
        public double SaldoAtual
        {
            get
            {
                return _saldoAtual;
            }
        }
        public double Limite
        {
            get
            {
                return _limite;
            }
        }
        public double GetSaldoDisponivel()
        {
            return _limite + _saldoAtual;
        }
        public double GetLimite()
        {
            return _limite;
        }
        public void AlterarLimiteCredito(double valor)
        {
            var log = new Log()
            {
                Data = DateTime.Now,
                Historico = "ALTERAÇÃO DE LIMITE DE CRÉDITO",
                IdVendedor = Vendedor.Id,
                LimiteCreditoAtual = valor,
                LimiteCreditoAnterior = _limite,
                SaldoAnterior = _saldoAtual,
                SaldoAtual = _saldoAtual
            };

            if (valor < 0)
            {
                throw new ArgumentException("Valor incorreto para lançamento", nameof(valor));
            }

            _limite = valor;

            Logs.Add(log);

        }
        public void LancarValor(Pedido pedido, ItemPedido itemPedido)
        {
            var Valor = ((itemPedido.PrecoVenda - itemPedido.PrecoBaseRCA) * itemPedido.Quantidade);

            var log = new Log()
            {
                Data = DateTime.Now,
                Historico = "LANÇAMENTO DE VALOR",
                IdVendedor = Vendedor.Id,
                LimiteCreditoAtual = _limite,
                LimiteCreditoAnterior = _limite,
                SaldoAnterior = _saldoAtual,
                SaldoAtual = (_saldoAtual + Valor),
                Quantidade = itemPedido.Quantidade,
                Seq = itemPedido.Seq,
                IdProduto = itemPedido.IdProduto
            };

            validarInformacao(itemPedido);

            if (Valor > GetSaldoDisponivel())
            {
                throw new Exception("Saldo insuficiente");
            }

            if (Valor != 0)
            {
                _saldoAtual -= Valor;
            }

            Logs.Add(log);

        }

        private static void validarInformacao(ItemPedido itemPedido)
        {
            if (itemPedido.PrecoVenda <= 0)
            {
                throw new ArgumentException("Valor incorreto para lançamento", nameof(itemPedido.PrecoVenda));
            }

            if (itemPedido.PrecoBaseRCA <= 0)
            {
                throw new ArgumentException("Valor incorreto para lançamento", nameof(itemPedido.PrecoBaseRCA));
            }

            if (itemPedido.Quantidade <= 0)
            {
                throw new ArgumentException("Valor incorreto para lançamento", nameof(itemPedido.Quantidade));
            }
        }

        public void TransferirSaldo(ContaCorrente contaDestino, double valor)
        {
            var log = new Log()
            {
                Data = DateTime.Now,
                Historico = "TRANSFERÊNCIA DE SALDO ENTRE RCA's",
                IdVendedor = contaDestino.Vendedor.Id,
                LimiteCreditoAtual = contaDestino._limite,
                LimiteCreditoAnterior = contaDestino._limite,
                SaldoAnterior = contaDestino._saldoAtual,
                SaldoAtual = contaDestino._saldoAtual + valor,
            };

            if (valor > GetSaldoDisponivel())
            {
                throw new Exception("Saldo insuficiente para está operação");
            }

            _saldoAtual -= valor;
            contaDestino._saldoAtual += valor;
            contaDestino.Logs.Add(log);
        }

    }

    public class Program
    {
        static void Main(string[] args)
        {
            ProcessaCenarios();            
            Console.ReadLine();
        }

        private static void ProcessaCenarios()
        {
            var vendedor = new Vendedor()
            {
                Id = 1,
                Nome = "Thiago"
            };
            var contaCorrente = new ContaCorrente(vendedor);
            contaCorrente.AlterarLimiteCredito(500);
            var produto1 = new Produto()
            {
                Id = 1,
                Descricao = "Coca Cola",
                PrecoTabela = 100
            };
            var produto2 = new Produto()
            {
                Id = 2,
                Descricao = "Sukita",
                PrecoTabela = 100
            };
            var produto3 = new Produto()
            {
                Id = 3,
                Descricao = "Fanta Uva",
                PrecoTabela = 100
            };
            var pedido = new Pedido(1, vendedor);

            //Lançamento de item 01
            ImprimirSaldoTela(contaCorrente);
            var itemPedido1 = new ItemPedido(pedido, produto1, 1, 10, 90, 100);
            contaCorrente.LancarValor(pedido, itemPedido1);
            pedido.ItensPedido.Add(itemPedido1);

            //Lançamento de item 02
            ImprimirSaldoTela(contaCorrente);
            var itemPedido2 = new ItemPedido(pedido, produto2, 2, 5, 100, 90);
            contaCorrente.LancarValor(pedido, itemPedido2);
            pedido.ItensPedido.Add(itemPedido2);

            //Lançamento de item 03
            ImprimirSaldoTela(contaCorrente);
            var itemPedido3 = new ItemPedido(pedido, produto3, 2, 5, 90, 90);
            contaCorrente.LancarValor(pedido, itemPedido3);
            pedido.ItensPedido.Add(itemPedido3);

            Console.WriteLine("Total pedido: {0}",pedido.TotalPedido.ToString("C"));

            //Lançamento 05 - Transferência de saldo
            var vendedor2 = new Vendedor()
            {
                Id = 2,
                Nome = "João"
            };
            var contaCorrente2 = new ContaCorrente(vendedor2);
            contaCorrente2.AlterarLimiteCredito(500);
            contaCorrente2.TransferirSaldo(contaCorrente, 100);
            ImprimirSaldoTela(contaCorrente);

            //Lançamento 06 - Alteração de límite de crédito para menos
            contaCorrente.AlterarLimiteCredito(400);
            ImprimirSaldoTela(contaCorrente);            

            Console.ReadLine();
        }
        private static void ImprimirSaldoTela(ContaCorrente contaCorrente)
        {            
            ImprimirLogs(contaCorrente);
            Console.WriteLine();
            Console.WriteLine("Vendedor: {0}\tCrédito: {1}\tSaldo atual {2}\tSaldo disponível {3}",
                                contaCorrente.Vendedor.Nome,
                                contaCorrente.Limite.ToString("C"),
                                contaCorrente.SaldoAtual.ToString("C"),
                                contaCorrente.GetSaldoDisponivel().ToString("C"));
            
            Console.WriteLine();
        }
        private static void ImprimirLogs(ContaCorrente contaCorrente)
        {
            Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t",
                              "Data".PadRight(20),
                              "L. Crédito At.".PadRight(10),
                              "L. Crédito Ant.".PadRight(10),
                              "Sd. At.".PadRight(10),
                              "Sd. Ant.".PadRight(10),
                              "Histórico".PadRight(20)
                              );

            foreach (var item in contaCorrente.Logs)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t",
                                  item.Data.ToString().PadRight(20),
                                  item.LimiteCreditoAtual.ToString("C").PadRight(10),
                                  item.LimiteCreditoAnterior.ToString("C").PadRight(10),
                                  item.SaldoAtual.ToString("C").PadRight(10),
                                  item.SaldoAnterior.ToString("C").PadRight(10),
                                  item.Historico.PadRight(20)
                                  );
            }
        }
    }
}


