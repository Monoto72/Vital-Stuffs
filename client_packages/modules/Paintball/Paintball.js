let browser;

mp.keys.bind(0x59, false, () => {
    if (!browser) {
        mp.events.callRemote('Paintball:Interact');
    }
});

mp.events.add('Paintball:PromptMenu', (gameInstances) => {
    if (!browser) {
        mp.gui.cursor.visible = true;
        browser = mp.browsers.new(`package://webpages/Paintball/index.html`);
        mp.gui.chat.push(JSON.stringify(gameInstances));
        browser.execute(`populateGames(${JSON.stringify(gameInstances)})`);
    }
});

mp.events.add('Paintball:CloseMenu', () => {
    if (browser) {
        mp.gui.cursor.visible = false;
        browser.destroy();
        browser = null;
    }
});

mp.events.add('Paintball:CreateGame', (map, mode, max, score, password) => {
    mp.events.callRemote('Paintball:CreateGame', map, mode, max, score, password);
    browser.reload(true);
});

mp.events.add('Paintball:ShowSession', (id) => {
    mp.events.callRemote('Paintball:ShowSession', id);
});

mp.events.add('debug', (msg) => {
    mp.gui.chat.push(msg);

});