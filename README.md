# AttackDefenseRunner
Runner for A&amp;D CTFs

# Structure
1. Give ADR the flag regex and the available servers
2. Docker image uploading
    1. Choose server to run the docker image
        1. Every docker image should ideally have it's own server
        2. If there are not enough servers available, distribute the docker images as good as possible
    2. Make that server run the docker image
    3. Get output back in input channel
    4. Filter the output of the docker image with a given regex
    5. Send flags back with rabbitmq
3. Log everything
