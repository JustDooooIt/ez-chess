using System;
using Godot;
using Godot.Collections;
using Godot.NativeInterop;

public static class CryptoUtils
{
  // ---------- 小工具 ----------
  private static byte[] ToBytes(Variant v)
    => System.Text.Encoding.UTF8.GetBytes(Json.Stringify(v, "", false));

  private static Variant FromBytes(byte[] bytes)
      => Json.ParseString(System.Text.Encoding.UTF8.GetString(bytes));

  private static string B64(byte[] data) => Marshalls.RawToBase64(data);
  private static byte[] UnB64(string s) => Marshalls.Base64ToRaw(s);

  // ---------- 加密（多收件人） + 可选签名 ----------
  // plaintextObj: 任意可 JSON 化的 Godot Variant（Dictionary/Array/...）
  // recipientPubKeyPaths: 多收件人公钥路径（如 "user://bob.pub"）
  // senderPrivKeyPath: 可为 ""（不签名）
  public static Dictionary EncryptFor(
      Variant plaintextObj,
      string[] recipientPubKeyPaths,
      string senderPrivKeyPath = ""
  )
  {
    var crypto = new Crypto();
    var plaintext = ToBytes(plaintextObj);

    // 1) 一次性会话材料：AES-256 key(32) + HMAC key(32) = 64B
    var keymat = crypto.GenerateRandomBytes(64);
    var aesKey = Slice(keymat, 0, 32);
    var macKey = Slice(keymat, 32, 32);
    var iv = crypto.GenerateRandomBytes(16);

    // 2) AES-CBC 加密正文
    var aes = new AesContext();
    aes.Start(AesContext.Mode.CbcEncrypt, aesKey, iv);
    var ct = aes.Update(plaintext);
    aes.Finish();

    // 3) HMAC-SHA256（对 iv||ct）
    var concat = Concat(iv, ct);
    var tag = crypto.HmacDigest(HashingContext.HashType.Sha256, macKey, concat);

    // 4) 为每个收件人用其公钥加密 keymat
    var recipients = new Array<Dictionary>();
    foreach (var pubPath in recipientPubKeyPaths)
    {
      var pub = new CryptoKey();
      var err = pub.Load(pubPath, true); // publicOnly=true
      if (err != Error.Ok)
        throw new Exception($"Load pubkey failed: {pubPath}");

      var ek = crypto.Encrypt(pub, keymat); // RSA-OAEP
      recipients.Add(new Dictionary {
                { "kid", System.IO.Path.GetFileName(pubPath) },
                { "ek", B64(ek) }
            });
    }

    var bundle = new Dictionary {
            { "enc", "RSA-OAEP + AES-256-CBC + HMAC-SHA256" },
            { "iv",  B64(iv) },
            { "ct",  B64(ct) },
            { "tag", B64(tag) },
            { "recipients", recipients }
        };

    // 5) 可选：对明文做 RSA-SHA256 签名
    if (!string.IsNullOrEmpty(senderPrivKeyPath))
    {
      var sk = new CryptoKey();
      if (sk.Load(senderPrivKeyPath) != Error.Ok)
        throw new Exception($"Load private key failed: {senderPrivKeyPath}");

      var sig = crypto.Sign(HashingContext.HashType.Sha256, plaintext, sk);
      bundle["sig"] = new Dictionary {
                { "kid", System.IO.Path.GetFileName(senderPrivKeyPath) },
                { "alg", "RSA-SHA256" },
                { "val", B64(sig) }
            };
    }

    return bundle;
  }

  // ---------- 解密 + 可选验签 ----------
  // optionalSenderPubKeyPath: 为空则不验签
  public static Variant DecryptWith(
      Dictionary bundle,
      string myPrivateKeyPath,
      string optionalSenderPubKeyPath = ""
  )
  {
    var crypto = new Crypto();
    var sk = new CryptoKey();
    if (sk.Load(myPrivateKeyPath) != Error.Ok)
      throw new Exception($"Load private key failed: {myPrivateKeyPath}");

    var iv = UnB64((string)bundle["iv"]);
    var ct = UnB64((string)bundle["ct"]);
    var tag = UnB64((string)bundle["tag"]);

    // 1) 从 recipients 中尝试解出 keymat，并校验 HMAC
    var recips = bundle["recipients"].AsGodotArray<Dictionary>();
    byte[] keymat = [];
    foreach (Dictionary r in recips)
    {
      var ek = UnB64((string)r["ek"]);
      var maybe = crypto.Decrypt(sk, ek);          // RSA-OAEP 解密
      if (maybe.Length == 64)
      {
        var macKey = Slice(maybe, 32, 32);
        var concat = Concat(iv, ct);
        var calcTag = crypto.HmacDigest(HashingContext.HashType.Sha256, macKey, concat);
        if (calcTag == tag)
        {
          keymat = maybe;
          break;
        }
      }
    }
    if (keymat.IsEmpty())
      throw new Exception("No matching recipient or HMAC verification failed.");

    // 2) AES-CBC 解密正文
    var aesKey = Slice(keymat, 0, 32);
    var aes = new AesContext();
    aes.Start(AesContext.Mode.CbcDecrypt, aesKey, iv);
    var pt = aes.Update(ct);
    aes.Finish();

    // 3) 可选：验签（对“明文”）
    if (bundle.ContainsKey("sig") && !string.IsNullOrEmpty(optionalSenderPubKeyPath))
    {
      var sigInfo = (Dictionary)bundle["sig"];
      var pub = new CryptoKey();
      if (pub.Load(optionalSenderPubKeyPath, true) != Error.Ok)
        throw new Exception($"Load sender pubkey failed: {optionalSenderPubKeyPath}");

      var ok = crypto.Verify(
          HashingContext.HashType.Sha256,
          pt, UnB64((string)sigInfo["val"]), pub
      );
      if (!ok) throw new Exception("Signature verify failed.");
    }

    return FromBytes(pt);
  }

  private static byte[] Slice(byte[] source, int offset, int length)
  {
    var dest = new byte[length];
    System.Array.Copy(source, offset, dest, 0, length);
    return dest;
  }

  private static byte[] Concat(byte[] a, byte[] b)
  {
    var result = new byte[a.Length + b.Length];
    Buffer.BlockCopy(a, 0, result, 0, a.Length);
    Buffer.BlockCopy(b, 0, result, a.Length, b.Length);
    return result;
  }
}
