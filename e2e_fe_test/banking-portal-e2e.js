#!/usr/bin/env node

/**
 * Test E2E per Banking Portal
 * Replica i passi eseguiti nel test manuale:
 * 1. Navigazione a http://localhost:4200 con viewport mobile
 * 2. Login con utente demo Mario Rossi
 * 3. Click su icona Trasferimento P2P
 * 4. Verifica URL finale
 * 
 * Ritorna: process.exit(1) per successo, process.exit(0) per fallimento
 * Uso: node banking-portal-e2e.js [--headless]
 */

const puppeteer = require('puppeteer');

// Configurazione del test
const CONFIG = {
  BASE_URL: 'http://localhost:4200',
  DEMO_USER: {
    fiscalCode: 'RSSMRA80A01H501Z', // Mario Rossi
    password: '1234'
  },
  VIEWPORT: {
    width: 375,
    height: 812
  },
  TIMEOUTS: {
    navigation: 30000,
    elementWait: 10000
  },
  EXPECTED_P2P_URL: 'http://localhost:4200/p2p-transfer'
};

/**
 * Funzione di logging con timestamp
 */
function log(message, type = 'INFO') {
  const timestamp = new Date().toISOString();
  console.log(`[${timestamp}] ${type}: ${message}`);
}

/**
 * Gestione degli errori con screenshot
 */
async function handleError(page, error, step) {
  try {
    const screenshotPath = `./error-${step}-${Date.now()}.png`;
    await page.screenshot({ path: screenshotPath, fullPage: true });
    log(`Screenshot salvato: ${screenshotPath}`, 'ERROR');
  } catch (screenshotError) {
    log(`Impossibile salvare screenshot: ${screenshotError.message}`, 'ERROR');
  }
  
  log(`ERRORE in ${step}: ${error.message}`, 'ERROR');
  throw error;
}

/**
 * Step 1: Navigazione alla pagina di login
 */
async function navigateToLogin(page) {
  log('Step 1: Navigazione alla pagina di login...');
  
  try {
    await page.goto(CONFIG.BASE_URL, { 
      waitUntil: 'networkidle2',
      timeout: CONFIG.TIMEOUTS.navigation 
    });
    
    // Verifica che la pagina di login sia caricata
    await page.waitForSelector('input', { timeout: CONFIG.TIMEOUTS.elementWait });
    
    log('✅ Navigazione completata con successo');
    return true;
  } catch (error) {
    await handleError(page, error, 'navigateToLogin');
  }
}

/**
 * Step 2: Login con utente demo
 */
async function performLogin(page) {
  log('Step 2: Esecuzione login...');
  
  try {
    // Inserimento codice fiscale (primo campo input)
    const fiscalCodeInput = await page.waitForSelector('input:first-of-type', { 
      timeout: CONFIG.TIMEOUTS.elementWait 
    });
    // Pulisce il campo selezionando tutto e sostituendo
    await fiscalCodeInput.click({ clickCount: 3 });
    await fiscalCodeInput.type(CONFIG.DEMO_USER.fiscalCode);
    log(`Codice fiscale inserito: ${CONFIG.DEMO_USER.fiscalCode}`);
    
    // Inserimento password (secondo campo input)
    const passwordInput = await page.waitForSelector('input[placeholder*="password"], input[type="password"], input:nth-of-type(2)', { 
      timeout: CONFIG.TIMEOUTS.elementWait 
    });
    // Pulisce il campo selezionando tutto e sostituendo
    await passwordInput.click({ clickCount: 3 });
    await passwordInput.type(CONFIG.DEMO_USER.password);
    log('Password inserita');
    
    // Click sul pulsante di login
    const loginButton = await page.waitForSelector('button', { 
      timeout: CONFIG.TIMEOUTS.elementWait 
    });
    await loginButton.click();
    log('Click sul pulsante Accedi');
    
    // Attesa caricamento home page
    await page.waitForNavigation({ 
      waitUntil: 'networkidle2',
      timeout: CONFIG.TIMEOUTS.navigation 
    });
    
    // Verifica presenza del saluto utente
    await page.waitForFunction(
      () => document.body.textContent.includes('Benvenuto, Mario Rossi'),
      { timeout: CONFIG.TIMEOUTS.elementWait }
    );
    
    log('✅ Login completato con successo');
    return true;
  } catch (error) {
    await handleError(page, error, 'performLogin');
  }
}

/**
 * Step 3: Click su icona Trasferimento P2P
 */
async function clickP2PIcon(page) {
  log('Step 3: Click su icona Trasferimento P2P...');
  
  try {
    // Scroll per trovare la sezione servizi
    await page.evaluate(() => {
      window.scrollTo(0, document.body.scrollHeight);
    });
    
    // Attesa che la sezione servizi sia visibile
    await page.waitForFunction(
      () => document.body.textContent.includes('Servizi bancari'),
      { timeout: CONFIG.TIMEOUTS.elementWait }
    );
    
    // Attesa che ci sia effettivamente un bottone/elemento cliccabile per P2P
    log('Attesa elemento cliccabile Trasferimento P2P...');
    await page.waitForFunction(
      () => {
        // Cerca elementi cliccabili che contengono "Trasferimento P2P"
        const clickableSelectors = ['button', 'a', '[role="button"]', '[onclick]', '.clickable'];
        
        for (const selector of clickableSelectors) {
          const elements = document.querySelectorAll(selector);
          for (const element of elements) {
            if (element.textContent && 
                element.textContent.includes('Trasferimento P2P') &&
                element.offsetParent !== null) { // Verifica che sia visibile
              return true;
            }
          }
        }
        
        // Verifica anche div/span con event listeners o che sono parent di elementi con testo P2P
        const allElements = document.querySelectorAll('*');
        for (const element of allElements) {
          if (element.textContent && 
              element.textContent.includes('Trasferimento P2P') &&
              element.offsetParent !== null &&
              (element.onclick || 
               element.style.cursor === 'pointer' ||
               element.getAttribute('role') === 'button' ||
               element.tagName === 'BUTTON' ||
               element.tagName === 'A')) {
            return true;
          }
        }
        
        return false;
      },
      { timeout: CONFIG.TIMEOUTS.elementWait }
    );
    
    log('✅ Elemento cliccabile P2P trovato e pronto!');
    
    // Click diretto sull'elemento cliccabile trovato
    const clickResult = await page.evaluate(() => {
      // Usa la stessa logica di ricerca del waitForFunction
      const clickableSelectors = ['button', 'a', '[role="button"]', '[onclick]', '.clickable'];
      
      // Prima prova con selettori specifici
      for (const selector of clickableSelectors) {
        const elements = document.querySelectorAll(selector);
        for (const element of elements) {
          if (element.textContent && 
              element.textContent.includes('Trasferimento P2P') &&
              element.offsetParent !== null) {
            element.click();
            return { success: true, method: 'selector', tag: element.tagName };
          }
        }
      }
      
      // Se non trovato, cerca tra tutti gli elementi
      const allElements = document.querySelectorAll('*');
      for (const element of allElements) {
        if (element.textContent && 
            element.textContent.includes('Trasferimento P2P') &&
            element.offsetParent !== null &&
            (element.onclick || 
             element.style.cursor === 'pointer' ||
             element.getAttribute('role') === 'button' ||
             element.tagName === 'BUTTON' ||
             element.tagName === 'A')) {
          element.click();
          return { success: true, method: 'generic', tag: element.tagName };
        }
      }
      
      return { success: false, error: 'Elemento cliccabile non trovato per il click' };
    });
    
    if (!clickResult.success) {
      throw new Error(clickResult.error);
    }
    
    log(`Click eseguito con successo (${clickResult.method}: ${clickResult.tag})`);
    
    log('Click eseguito su Trasferimento P2P');
    
    // Attesa navigazione alla pagina P2P
    await page.waitForNavigation({ 
      waitUntil: 'networkidle2',
      timeout: CONFIG.TIMEOUTS.navigation 
    });
    
    log('✅ Navigazione alla pagina P2P completata');
    return true;
  } catch (error) {
    await handleError(page, error, 'clickP2PIcon');
  }
}

/**
 * Step 4: Verifica URL finale
 */
async function verifyP2PPage(page) {
  log('Step 4: Verifica pagina P2P...');
  
  try {
    const currentUrl = page.url();
    log(`URL attuale: ${currentUrl}`);
    
    if (currentUrl !== CONFIG.EXPECTED_P2P_URL) {
      throw new Error(`URL non corrispondente. Atteso: ${CONFIG.EXPECTED_P2P_URL}, Ottenuto: ${currentUrl}`);
    }
    
    // Verifica presenza elementi caratteristici della pagina P2P
    await page.waitForFunction(
      () => document.body.textContent.includes('Trasferimento P2P') && 
            document.body.textContent.includes('Destinatario'),
      { timeout: CONFIG.TIMEOUTS.elementWait }
    );
    
    log('✅ Pagina P2P verificata con successo');
    log(`✅ URL corretto: ${currentUrl}`);
    return true;
  } catch (error) {
    await handleError(page, error, 'verifyP2PPage');
  }
}

/**
 * Funzione principale del test
 */
async function runE2ETest() {
  const startTime = Date.now();
  log('🚀 Avvio test E2E Banking Portal');
  
  // Parsing argomenti comando
  const isHeadless = process.argv.includes('--headless');
  log(`Modalità: ${isHeadless ? 'headless' : 'headed'}`);
  
  let browser = null;
  let page = null;
  
  try {
    // Avvio browser
    log('Avvio browser Chromium...');
    browser = await puppeteer.launch({
      headless: isHeadless,
      args: [
        '--window-size=375,812',
        '--disable-web-security',
        '--disable-features=VizDisplayCompositor',
        '--no-sandbox',
        '--disable-setuid-sandbox'
      ],
      defaultViewport: CONFIG.VIEWPORT
    });
    
    page = await browser.newPage();
    
    // Configurazione viewport mobile
    await page.setViewport(CONFIG.VIEWPORT);
    
    // Configurazione timeout default
    page.setDefaultTimeout(CONFIG.TIMEOUTS.elementWait);
    
    log('Browser avviato con successo');
    
    // Esecuzione step del test
    await navigateToLogin(page);
    await performLogin(page);
    await clickP2PIcon(page);
    await verifyP2PPage(page);
    
    const endTime = Date.now();
    const duration = ((endTime - startTime) / 1000).toFixed(2);
    
    log(`🎉 TEST E2E COMPLETATO CON SUCCESSO in ${duration}s`);
    log('Tutti gli step sono stati eseguiti correttamente');
    
    // Successo: ritorna 1
    process.exit(1);
    
  } catch (error) {
    const endTime = Date.now();
    const duration = ((endTime - startTime) / 1000).toFixed(2);
    
    log(`❌ TEST E2E FALLITO dopo ${duration}s`, 'ERROR');
    log(`Errore: ${error.message}`, 'ERROR');
    
    // Screenshot finale in caso di errore
    if (page) {
      try {
        await page.screenshot({ 
          path: `./final-error-${Date.now()}.png`, 
          fullPage: true 
        });
      } catch (screenshotError) {
        log(`Impossibile salvare screenshot finale: ${screenshotError.message}`, 'ERROR');
      }
    }
    
    // Fallimento: ritorna 0
    process.exit(0);
    
  } finally {
    // Cleanup
    if (browser) {
      await browser.close();
      log('Browser chiuso');
    }
  }
}

// Avvio del test se questo file viene eseguito direttamente
if (require.main === module) {
  runE2ETest();
}

module.exports = { runE2ETest }; 