$(function () {
    $('#btFacebook').on('click', function (e) {
        e.preventDefault();
        FB.login(function (response) {
            if (response.authResponse) {
                acessoAPI(response.authResponse.accessToken);
            }
        }, { scope: 'publish_actions,read_stream, publish_stream, photo_upload, email, user_photos, user_photo_video_tags, public_profile' });
    });
});

(function (d) {
    var js, id = 'facebook-jssdk', ref = d.getElementsByTagName('script')[0];
    if (d.getElementById(id)) { return; }
    js = d.createElement('script'); js.id = id; js.async = true;
    js.src = "//connect.facebook.net/en_US/all.js?appId=1513171988905615";
    ref.parentNode.insertBefore(js, ref);
} (document));

window.fbAsyncInit = function () {

    //FB.login();

    FB.init({
        appId: '1513171988905615',                        // App ID from the app dashboard
        channelUrl: 'http://www.futmanager.net/channel.html', // Channel file for x-domain comms
        status: true,                                 // Check Facebook Login status
        xfbml: true                                  // Look for social plugins on the page
    });

    FB.Event.subscribe('auth.authResponseChange', function (response) {
        if (response.status === 'connected') {
            acessoAPI(response.authResponse.accessToken);
        } else {
            FB.login(function (response) {
                if (response.authResponse) {
                    //window.location = 'Passo2.html';
                }
            }, { scope: 'publish_actions,read_stream, publish_stream, photo_upload, email, user_photos, user_photo_video_tags, public_profile' });
        }
    });
};

function acessoAPI(accessToken) {
    //console.log(accessToken);
    //https://graph.facebook.com/oauth/access_token?client_id=APP_ID&client_secret=APP_SECRET&grant_type=fb_exchange_token&fb_exchange_token=EXISTING_ACCESS_TOKEN 
    //console.log('Welcome!  Fetching your information.... ');
    var iduser = '';
    FB.api('/me', { fields: 'name,id,picture,email' }, function (response) {
        $("#left-side").prepend("<div class='me'><h4>1. ESCOLHA OS JOGADORES </h4><img src='" + response.picture.data.url + "' style='float:left;' /> <span>Amigos de <br /><b>" + response.name + "</b></span></div>");
        iduser = response.id;
        sessao = iduser;

        $.ajax({
            type: "POST",
            url: 'Conta/CadastroFacebook/',
            data: { nome: response.name, id: response.id, email: response.email },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                if (result != "ok") {
                    alert('Erro ao tentar mudar a situação do jogador.');
                }
            }
        });
    });    
}