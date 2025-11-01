package com.salman_ecommerce.category_template_service.grpc;

import org.springframework.stereotype.Component;

import com.salman_ecommerce.category_template_service.mapper.TemplateMapper;

import io.grpc.Status;
import io.grpc.stub.StreamObserver;
import net.devh.boot.grpc.server.service.GrpcService;

@GrpcService
@Component
public class CategoryTemplateGrpcService extends CategoryTemplateServiceGrpc.CategoryTemplateServiceImplBase {

    private final com.salman_ecommerce.category_template_service.service.TemplateService templateService;

    public CategoryTemplateGrpcService(com.salman_ecommerce.category_template_service.service.TemplateService templateService) {
        this.templateService = templateService;
    }

    @Override
    public void getTemplateById(GetTemplateByIdRequest request, StreamObserver<GetTemplateByIdResponse> responseObserver) {
        try {
            var dto = templateService.getTemplateById(request.getId());
            var resp = GetTemplateByIdResponse.newBuilder().setTemplate(TemplateMapper.toProto(dto)).build();
            responseObserver.onNext(resp);
            responseObserver.onCompleted();
        } catch (Exception ex) {
            responseObserver.onError(Status.NOT_FOUND.withDescription(ex.getMessage()).asRuntimeException());
        }
    }
}
