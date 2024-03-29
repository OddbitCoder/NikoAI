# Generated by the gRPC Python protocol compiler plugin. DO NOT EDIT!
"""Client and server classes corresponding to protobuf-defined services."""
import grpc

import contracts_pb2 as contracts__pb2


class ModelServiceStub(object):
    """Missing associated documentation comment in .proto file."""

    def __init__(self, channel):
        """Constructor.

        Args:
            channel: A grpc.Channel.
        """
        self.Process = channel.unary_unary(
                '/ModelService/Process',
                request_serializer=contracts__pb2.ProcessRequest.SerializeToString,
                response_deserializer=contracts__pb2.ProcessReply.FromString,
                )


class ModelServiceServicer(object):
    """Missing associated documentation comment in .proto file."""

    def Process(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')


def add_ModelServiceServicer_to_server(servicer, server):
    rpc_method_handlers = {
            'Process': grpc.unary_unary_rpc_method_handler(
                    servicer.Process,
                    request_deserializer=contracts__pb2.ProcessRequest.FromString,
                    response_serializer=contracts__pb2.ProcessReply.SerializeToString,
            ),
    }
    generic_handler = grpc.method_handlers_generic_handler(
            'ModelService', rpc_method_handlers)
    server.add_generic_rpc_handlers((generic_handler,))


 # This class is part of an EXPERIMENTAL API.
class ModelService(object):
    """Missing associated documentation comment in .proto file."""

    @staticmethod
    def Process(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(request, target, '/ModelService/Process',
            contracts__pb2.ProcessRequest.SerializeToString,
            contracts__pb2.ProcessReply.FromString,
            options, channel_credentials,
            insecure, call_credentials, compression, wait_for_ready, timeout, metadata)
