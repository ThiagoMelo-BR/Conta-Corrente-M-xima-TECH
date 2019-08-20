using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContaCorrenteMaximaTECH;

namespace TDD_ContaCorrente
{
    [TestClass]
    public class TDDContaCorrente
    {
        private ContaCorrente contaCorrente;
        private Vendedor vendedor;
        private Produto produto1;
        private Produto produto2;
        private Produto produto3;
        private Pedido pedido;

        public TDDContaCorrente()
        {
            vendedor = new Vendedor()
            {
                Id = 1,
                Nome = "Thiago"
            };
            contaCorrente = new ContaCorrente(vendedor);
            produto1 = new Produto()
            {
                Id = 1,
                Descricao = "Coca Cola",
                PrecoTabela = 100
            };
            produto2 = new Produto()
            {
                Id = 2,
                Descricao = "Sukita",
                PrecoTabela = 100
            };
            produto3 = new Produto()
            {
                Id = 3,
                Descricao = "Fanta Uva",
                PrecoTabela = 100
            };
            pedido = new Pedido(1, vendedor);
        }
        [TestMethod]
        public void AlterarLimiteCreditoMais()
        {
            contaCorrente.AlterarLimiteCredito(500);
            Assert.AreEqual(500, contaCorrente.GetSaldoDisponivel(), "Valor esperado igual a 500 ap�s a primeiro lan�amento de cr�dito");
        }
        [TestMethod]
        public void LancamentoDebito()
        {
            //Lan�amento de d�bito no valor de 100,00   
            contaCorrente.AlterarLimiteCredito(500);
            var itemPedido1 = new ItemPedido(pedido, produto1, 1, 10, 90, 100);
            contaCorrente.LancarValor(pedido, itemPedido1);
            pedido.ItensPedido.Add(itemPedido1);
            Assert.AreEqual(400, contaCorrente.GetSaldoDisponivel(), "Valor esperado igual a 400 ap�s a primeira movimenta��o");
        }
        [TestMethod]
        public void LancamentoCredito()
        {
            //Lan�amento de cr�dito no valor de 50,00  
            LancamentoDebito();
            var itemPedido2 = new ItemPedido(pedido, produto2, 2, 5, 100, 90);
            contaCorrente.LancarValor(pedido, itemPedido2);
            pedido.ItensPedido.Add(itemPedido2);
            Assert.AreEqual(450, contaCorrente.GetSaldoDisponivel(), "Valor esperado igual a 450 ap�s a primeira movimenta��o");
        }
        [TestMethod]
        public void LancamentoQueNaoMovimentarCC()
        {
            LancamentoCredito();
            //Lan�amento que n�o movimenta o conta corrente            
            var itemPedido3 = new ItemPedido(pedido, produto3, 2, 5, 90, 90);
            contaCorrente.LancarValor(pedido, itemPedido3);
            pedido.ItensPedido.Add(itemPedido3);
            Assert.AreEqual(450, contaCorrente.GetSaldoDisponivel(), "Valor esperado igual a 450 ap�s a primeira movimenta��o");
        }
        [TestMethod]
        public void TransferenciaDeSaldoEntreVendedores()
        {
            LancamentoQueNaoMovimentarCC();
            //Lan�amento 05 - Transfer�ncia de saldo entre vendedores           
            var vendedor2 = new Vendedor()
            {
                Id = 2,
                Nome = "Jo�o"
            };
            var contaCorrente2 = new ContaCorrente(vendedor2);
            contaCorrente2.AlterarLimiteCredito(500);
            contaCorrente2.TransferirSaldo(contaCorrente, 100);
            Assert.AreEqual(550, contaCorrente.GetSaldoDisponivel(), "Valor esperado igual a 450 para o vendedor 1 ap�s transfer�ncia");
            Assert.AreEqual(400, contaCorrente2.GetSaldoDisponivel(), "Valor esperado igual a 400 para o vendedor 2 ap�s transfer�ncia");
        }
        [TestMethod]
        public void AlteracaoLimiteCreditoMenos()
        {
            TransferenciaDeSaldoEntreVendedores();
            contaCorrente.AlterarLimiteCredito(400);
            Assert.AreEqual(450, contaCorrente.GetSaldoDisponivel(), "Valor esperado igual a 450 para o vendedor 1 ap�s transfer�ncia");
        }
    }
}
