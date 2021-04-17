//
// Ipv4AddressToIpv6.m
// Create by Batcel on 2017-08-29

#import <sys/socket.h>
#import <netdb.h>
#import <arpa/inet.h>
#import <err.h>

#define MakeStringCopy( _x_ ) ( _x_ != NULL && [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL

#if defined (__cplusplus)
extern "C"  
{ 
#endif     
	const char* Ipv4ToIpv6(const char *SvrIp)
	{
		if( nil == SvrIp )
			return NULL;
		const char *newChar = "No";

		struct addrinfo* res0;
		struct addrinfo hints;
		struct addrinfo* res;
		int n, s;
	
		memset(&hints, 0, sizeof(hints));
	
		hints.ai_flags = AI_DEFAULT;
		hints.ai_family = PF_UNSPEC;
		hints.ai_socktype = SOCK_STREAM;
	
		if((n=getaddrinfo(SvrIp, "http", &hints, &res0))!=0)
		{
			printf("getaddrinfo error: %s\n",gai_strerror(n));
			return NULL;
		}
	
		struct sockaddr_in6* addr6;
		struct sockaddr_in* addr;
		NSString * IpAddrStr = NULL;
		char ipbuf[32];
		s = -1;
		for(res = res0; res; res = res->ai_next)
		{
			if (res->ai_family == AF_INET6)
			{
				addr6 =( struct sockaddr_in6*)res->ai_addr;
				newChar = inet_ntop(AF_INET6, &addr6->sin6_addr, ipbuf, sizeof(ipbuf));
				NSString * TempA = [[NSString alloc] initWithCString:(const char*)newChar encoding:NSASCIIStringEncoding];
				NSString * TempB = [NSString stringWithUTF8String:"|Ipv6"];
			
				IpAddrStr = [TempA stringByAppendingString: TempB];
				//printf("%s\n", newChar);
			}
			else
			{
				addr =( struct sockaddr_in*)res->ai_addr;
				newChar = inet_ntop(AF_INET, &addr->sin_addr, ipbuf, sizeof(ipbuf));
				NSString * TempA = [[NSString alloc] initWithCString:(const char*)newChar encoding:NSASCIIStringEncoding];
				NSString * TempB = [NSString stringWithUTF8String:"|Ipv4"];
			
				IpAddrStr = [TempA stringByAppendingString: TempB];			
				//printf("%s\n", newChar);
			}
			break;
		}
	
	
		freeaddrinfo(res0);
	
		//printf("getaddrinfo OK");
	
		return MakeStringCopy(IpAddrStr);
	}
#if defined (__cplusplus)
}
#endif